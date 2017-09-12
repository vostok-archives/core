using System.Collections.Generic;

namespace Vostok.Flow
{
    public static class IContextPropertiesExtensions_SetDistributedContext
    {
        public static void SetDistributedContext(this IContextProperties properties, IEnumerable<KeyValuePair<string, string>> candidates)
        {
            var serializer = Context.Configuration.Serializer;
            var keyDecoder = Context.Configuration.KeyDecoder;
            var distributedProperties = Context.Configuration.Properties;
            if (serializer == null || keyDecoder == null)
            {
                return;
            }

            foreach (var item in candidates)
            {
                var key = keyDecoder.Decode(item.Key);
                if (!distributedProperties.TryGetValue(key, out var type))
                {
                    continue;
                }

                if (!serializer.TryDeserialize(item.Value, type, out var value))
                {
                    continue;
                }

                properties.Set(key, value);
            }
        }
    }
}