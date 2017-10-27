using System;
using Vostok.Clusterclient.Topology;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Commons.Model;

namespace Vostok.Airlock
{
    public class AirlockConfig
    {
        public IClusterProvider ClusterProvider { get; set; }

        public string ApiKey { get; set; }

        public DataSize MaximumRecordSize { get; set; } = 1.Megabytes();

        public DataSize MaximumMemoryConsumption { get; set; } = 128.Megabytes();

        public DataSize MaximumBatchSizeToSend { get; set; } = 4.Megabytes();

        public DataSize InitialPooledBufferSize { get; set; } = 16.Kilobytes();

        public int InitialPooledBuffersCount { get; set; } = 32;

        public TimeSpan SendPeriod { get; set; } = 2.Seconds();

        public TimeSpan SendPeriodCap { get; set; } = 5.Minutes();

        public TimeSpan RequestTimeout { get; set; } = 30.Seconds();

        public bool EnableTracing { get; set; } = false;
    }
}