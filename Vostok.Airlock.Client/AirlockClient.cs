using System;
using System.Collections.Concurrent;
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
        private readonly DataSenderDaemon dataSenderDaemon;
        private readonly AtomicLong lostItemsCounter;
        private readonly AtomicLong sentItemsCounter;
        private readonly ILog log;

        private readonly AtomicBoolean isDisposed;
        private readonly AtomicBoolean pushAfterDisposeLogged;

        public AirlockClient(AirlockConfig config, ILog log = null)
        {
            AirlockConfigValidator.Validate(config);

            this.config = config;

            this.log = log = (log ?? new SilentLog()).ForContext(this);

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

            dataSenderDaemon = new DataSenderDaemon(dataSender, config, log);
            isDisposed = new AtomicBoolean(false);
            pushAfterDisposeLogged = new AtomicBoolean(false);
        }

        public long LostItemsCount => lostItemsCounter.Value;

        public long SentItemsCount => sentItemsCounter.Value;

        public void Push<T>(string routingKey, T item, DateTimeOffset? timestamp = null)
        {
            if (isDisposed)
            {
                LogPushAfterDispose(routingKey);
                return;
            }

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

        public void Dispose()
        {
            if (isDisposed.TrySetTrue())
            {
                dataSenderDaemon.Dispose();
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

        private void LogPushAfterDispose(string routingKey)
        {
            if (pushAfterDisposeLogged.TrySetTrue())
            {
                log.Warn($"Tried to push a message to routing key '{routingKey}' after dispose of AirlockClient");
            }
        }
    }
}