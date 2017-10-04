using System;

namespace Vostok.Airlock
{
    public class Airlock : IAirlock
    {
        private readonly AirlockConfig config;

        public Airlock(AirlockConfig config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public void Push<T>(string routingKey, T item)
        {
            if (!AirlockSerializerRegistry.TryGet<T>(out var serializer))
                return;

            throw new NotImplementedException();
        }
    }
}
