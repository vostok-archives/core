using System.Collections.Generic;
using Vstk.Flow.Serializers;

namespace Vstk.Flow
{
    public static class Context
    {
        private static readonly Serializer serializer;

        static Context()
        {
            Properties = new ContextProperties();
            Configuration = new ContextConfiguration();
            serializer = new Serializer();
        }

        public static IContextProperties Properties { get; }

        public static IContextConfiguration Configuration { get; }

        public static IEnumerable<KeyValuePair<string, string>> SerializeDistributedProperties()
        {
            foreach (var distributedProperty in Configuration.DistributedProperties)
            {
                if (!Properties.Current.TryGetValue(distributedProperty, out var value))
                {
                    continue;
                }

                if (!serializer.TrySerialize(value, out var stringValue))
                {
                    continue;
                }

                yield return new KeyValuePair<string, string>(distributedProperty, stringValue);
            }
        }

        public static void PopulateDistributedProperties(IEnumerable<KeyValuePair<string, string>> candidates)
        {
            foreach (var candidate in candidates)
            {
                if (!Configuration.DistributedProperties.Contains(candidate.Key))
                {
                    continue;
                }

                if (!serializer.TryDeserialize(candidate.Value, out var value))
                {
                    continue;
                }

                Properties.SetProperty(candidate.Key, value);
            }
        }
    }
}
