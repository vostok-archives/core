using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Commons.Synchronization;
using Vostok.Logging;
using Vostok.Logging.Logs;

namespace Vostok.Airlock
{
    public class AirlockClient : IAirlockClient, IDisposable
    {
        private readonly AirlockConfig config;
        private readonly MemoryManager memoryManager;
        private readonly RecordWriter recordWriter;
        private readonly ConcurrentDictionary<string, IBufferPool> bufferPools;
        private readonly FlushTracker flushTracker;
        private readonly DataSenderDaemon dataSenderDaemon;
        private readonly AtomicLong lostItemsCounter;
        private readonly AtomicLong sentItemsCounter;

        private readonly AtomicBoolean isDisposed;
        //@ezsilmar
        //Dispose should wait for all pending flushes
        private readonly ReaderWriterLockSlim flushDisposeSyncObject;

        public AirlockClient(AirlockConfig config, ILog log = null)
        {
            AirlockConfigValidator.Validate(config);

            this.config = config;

            log = (log ?? new SilentLog()).ForContext(this);
            memoryManager = new MemoryManager(
                config.MaximumMemoryConsumption.Bytes,
                (int) config.InitialPooledBufferSize.Bytes
            );
            recordWriter = new RecordWriter(new RecordSerializer(config.MaximumRecordSize, log));
            bufferPools = new ConcurrentDictionary<string, IBufferPool>();
            lostItemsCounter = new AtomicLong(0);
            sentItemsCounter = new AtomicLong(0);

            var requestSender = new RequestSender(config, log);
            var commonBatchBuffer = new byte[config.MaximumBatchSizeToSend.Bytes];
            var bufferSliceFactory = new BufferSliceFactory();
            var dataBatchesFactory = new DataBatchesFactory(
                bufferPools,
                bufferSliceFactory,
                commonBatchBuffer
            );
            var dataSender = new DataSender(
                dataBatchesFactory,
                requestSender,
                log,
                lostItemsCounter,
                sentItemsCounter
            );

            flushTracker = new FlushTracker();
            dataSenderDaemon = new DataSenderDaemon(dataSender, flushTracker, config, log);

            isDisposed = new AtomicBoolean(false);
            flushDisposeSyncObject = new ReaderWriterLockSlim();
        }

        public long LostItemsCount => lostItemsCounter.Value;

        public long SentItemsCount => sentItemsCounter.Value;

        public void Push<T>(string routingKey, T item, DateTimeOffset? timestamp = null)
        {
            if (isDisposed)
                return;

            if (!AirlockSerializerRegistry.TryGet<T>(out var serializer))
                return;

            if (!recordWriter.TryWrite(
                item,
                serializer,
                timestamp ?? DateTimeOffset.UtcNow,
                ObtainBufferPool(routingKey)))
            {
                lostItemsCounter.Increment();
            }
        }

        public Task FlushAsync()
        {
            if (isDisposed)
                return Task.CompletedTask;

            flushDisposeSyncObject.EnterReadLock();
            try
            {
                if (isDisposed)
                {
                    return Task.CompletedTask;
                }
                return flushTracker.RequestFlush();
            }
            finally
            {
                flushDisposeSyncObject.ExitReadLock();
            }
        }

        public void Dispose()
        {
            if (isDisposed.TrySetTrue())
            {
                flushDisposeSyncObject.EnterWriteLock();
                try
                {
                    flushTracker.RequestFlush().GetAwaiter().GetResult();
                    dataSenderDaemon.Dispose();
                }
                finally
                {
                    flushDisposeSyncObject.ExitWriteLock();
                }
            }
        }

        private IBufferPool ObtainBufferPool(string routingKey)
        {
            return bufferPools.GetOrAdd(
                routingKey,
                _ => new BufferPool(
                    memoryManager,
                    config.InitialPooledBuffersCount
                ));
        }
    }
}