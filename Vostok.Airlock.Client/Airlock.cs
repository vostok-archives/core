﻿using System;
using System.Collections.Concurrent;
using Vostok.Logging;
using Vostok.Logging.Logs;

namespace Vostok.Airlock
{
    public class Airlock : IAirlock, IDisposable
    {
        private readonly AirlockConfig config;
        private readonly MemoryManager memoryManager;
        private readonly RecordWriter recordWriter;
        private readonly ConcurrentDictionary<string, IBufferPool> bufferPools;
        private readonly DataSenderDaemon dataSenderDaemon;

        public Airlock(AirlockConfig config, ILog log = null)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            log = (log ?? new SilentLog()).ForContext(this);
            memoryManager = new MemoryManager(config.MaximumMemoryConsumption.Bytes, (int) config.InitialPooledBufferSize.Bytes);
            recordWriter = new RecordWriter(new RecordSerializer());
            bufferPools = new ConcurrentDictionary<string, IBufferPool>();

            var requestSender = new RequestSender(config, log);
            var commonBatchBuffer = new byte[config.MaximumBatchSizeToSend.Bytes];
            var dataBatchesFactory = new DataBatchesFactory(bufferPools, commonBatchBuffer, log);
            var dataSender = new DataSender(dataBatchesFactory, requestSender, log);

            dataSenderDaemon = new DataSenderDaemon(dataSender, config);
        }

        public void Push<T>(string routingKey, T item, DateTimeOffset? timestamp = null)
        {
            if (!AirlockSerializerRegistry.TryGet<T>(out var serializer))
                return;

            recordWriter.Write(item, serializer, timestamp ?? DateTimeOffset.UtcNow, ObtainBufferPool(routingKey));

            dataSenderDaemon.Start();
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
