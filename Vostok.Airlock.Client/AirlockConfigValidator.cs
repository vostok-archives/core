using System;

namespace Vostok.Airlock
{
    internal static class AirlockConfigValidator
    {
        public static void Validate(AirlockConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (config.ClusterProvider == null)
                throw new ArgumentNullException(nameof(config.ClusterProvider));

            if (string.IsNullOrEmpty(config.ApiKey))
                throw new ArgumentNullException(nameof(config.ApiKey));

            if (config.MaximumRecordSize > config.MaximumBatchSizeToSend)
                throw new ArgumentException($"Provided maximum record size ({config.MaximumRecordSize}) was greater than maximum batch size ({config.MaximumBatchSizeToSend}).");
        }
    }
}