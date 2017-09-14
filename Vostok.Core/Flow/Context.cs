﻿using System.Collections.Generic;
using Vostok.Flow.DistributedContextSerializer;

namespace Vostok.Flow
{
    public static class Context
    {
        private static readonly Serializer serializer;

        static Context()
        {
            Properties = new ContextProperties();
            Configuration = new ContextConfiguration();
            serializer = Serializer.Create();
        }

        public static IContextProperties Properties { get; }

        public static IContextConfiguration Configuration { get; }

        public static IEnumerable<KeyValuePair<string, string>> BuildDistributedContext()
        {
            var result = new Dictionary<string, string>();
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

                result[distributedProperty] = stringValue;
            }

            return result;
        }

        public static void SetDistributedContext(IEnumerable<KeyValuePair<string, string>> candidates)
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
