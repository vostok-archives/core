using System;
using System.Collections.Concurrent;
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
        private readonly DataSenderDaemon dataSenderDaemon;
        private readonly AtomicLong lostItemsCounter;

        public AirlockClient(AirlockConfig config, ILog log = null)
        {
            AirlockConfigValidator.Validate(config);

            this.config = config;

            log = (log ?? new SilentLog()).ForContext(this);
            memoryManager = new MemoryManager(config.MaximumMemoryConsumption.Bytes, (int) config.InitialPooledBufferSize.Bytes);
            recordWriter = new RecordWriter(new RecordSerializer(config.MaximumRecordSize, log));
            bufferPools = new ConcurrentDictionary<string, IBufferPool>();
            lostItemsCounter = new AtomicLong(0);

            var requestSender = new RequestSender(config, log);
            var commonBatchBuffer = new byte[config.MaximumBatchSizeToSend.Bytes];
            var bufferSliceFactory = new BufferSliceFactory();
            var dataBatchesFactory = new DataBatchesFactory(bufferPools, bufferSliceFactory, commonBatchBuffer);
            var dataSender = new DataSender(dataBatchesFactory, requestSender, log, lostItemsCounter);

            dataSenderDaemon = new DataSenderDaemon(dataSender, config, log);
        }

        public long LostItemsCount => lostItemsCounter.Value;

        public void Push<T>(string routingKey, T item, DateTimeOffset? timestamp = null)
        {
            if (!AirlockSerializerRegistry.TryGet<T>(out var serializer))
                return;

            if (recordWriter.TryWrite(item, serializer, timestamp ?? DateTimeOffset.UtcNow, ObtainBufferPool(routingKey)))
            {
                dataSenderDaemon.Start();
            }
            else
            {
                lostItemsCounter.Increment();
            }
        }

        public Task FlushAsync()
        {
            return dataSenderDaemon.FlushAsync();
        }

        public void Dispose()
        {
            dataSenderDaemon.Dispose();
        }

        private IBufferPool ObtainBufferPool(string routingKey)
        {
            return bufferPools.GetOrAdd(routingKey, _ => new BufferPool(memoryManager, config.InitialPooledBuffersCount));
        }
    }
}
