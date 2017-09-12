using System.Collections.Generic;

namespace Vostok.Flow
{
    public static class IContextPropertiesExtensions_BuildDistributedContext
    {
        public static IEnumerable<KeyValuePair<string, string>> BuildDistributedContext(this IContextProperties properties)
        {
            var serializer = Context.Configuration.Serializer;
            var keyDecoder = Context.Configuration.KeyDecoder;
            var distributedProperties = Context.Configuration.Properties;
            if (serializer == null || keyDecoder == null)
            {
                return null;
            }

            var result = new Dictionary<string, string>();
            foreach (var x in properties.Current)
            {
                if (!distributedProperties.TryGetValue(x.Key, out var type))
                {
                    continue;
                }

                if (x.Value.GetType() == type)
                {
                    continue;
                }

                if (serializer.TrySerialize(x.Value, out var serializedValue))
                {
                    continue;
                }

                var encodedKey = keyDecoder.Encode(x.Key);
                result.Add(encodedKey, serializedValue);
            }

            return result;
        }
    }
}