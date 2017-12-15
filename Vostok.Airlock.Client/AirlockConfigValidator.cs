using System;

namespace Vostok.Airlock
{
    internal static class AirlockConfigValidator
    {
        public static void Validate(AirlockConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (config.SendPeriod < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(config.SendPeriod));

            if (config.SendPeriod > config.SendPeriodCap)
                throw new ArgumentException($"{nameof(config.SendPeriod)} should be less or equal to {nameof(config.SendPeriodCap)}");

            if (config.ClusterProvider == null)
                throw new ArgumentNullException(nameof(config.ClusterProvider));

            if (string.IsNullOrEmpty(config.ApiKey))
                throw new ArgumentNullException(nameof(config.ApiKey));

            if (config.MaximumRecordSize > config.MaximumBatchSizeToSend)
                throw new ArgumentException($"Provided maximum record size ({config.MaximumRecordSize}) was greater than maximum batch size ({config.MaximumBatchSizeToSend}).");
        }
    }
}