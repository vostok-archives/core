using System;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Commons.Model;

namespace Vostok.Airlock
{
    public class AirlockConfig
    {
        public string ApiKey = "insert api key here";

        public DataSize MaximumMemoryConsumption { get; set; } = 128.Megabytes();

        public DataSize MaximumBatchSizeToSend { get; set; } = 4.Megabytes();

        public DataSize InitialPooledBufferSize { get; set; } = 32.Kilobytes();

        public int InitialPooledBuffersCount { get; set; } = 16;

        public TimeSpan SendPeriod { get; set; } = 5.Seconds();

        public TimeSpan SendPeriodCap { get; set; } = 5.Minutes();

        public TimeSpan RequestTimeout { get; set; } = 20.Seconds();
    }
}