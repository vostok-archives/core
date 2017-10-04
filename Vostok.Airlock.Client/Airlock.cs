using System;
using System.Collections.Concurrent;

namespace Vostok.Airlock
{
    public class Airlock : IAirlock
    {
        private readonly AirlockConfig config;
        private readonly MemoryManager memoryManager;
        private readonly RecordWriter recordWriter;
        private readonly ConcurrentDictionary<string, BufferPool> bufferPools;

        public Airlock(AirlockConfig config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            memoryManager = new MemoryManager(config.MaximumMemoryConsumption.Bytes, (int) config.InitialPooledBufferSize.Bytes);
            recordWriter = new RecordWriter(new RecordSerializer());
            bufferPools = new ConcurrentDictionary<string, BufferPool>();
        }

        public void Push<T>(string routingKey, T item, DateTimeOffset? timestamp = null)
        {
            if (!AirlockSerializerRegistry.TryGet<T>(out var serializer))
                return;

            recordWriter.Write(item, serializer, timestamp ?? DateTimeOffset.UtcNow, ObtainBufferPool(routingKey));
        }

        private BufferPool ObtainBufferPool(string routingKey)
        {
            return bufferPools.GetOrAdd(routingKey, _ => new BufferPool(memoryManager, config.InitialPooledBuffersCount));
        }
    }
}
