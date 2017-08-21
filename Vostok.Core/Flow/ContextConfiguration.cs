using System.Collections.Generic;
using System.Collections.Immutable;

namespace Vostok.Flow
{
    internal class ContextConfiguration : IContextConfiguration
    {
        private ImmutableHashSet<string> distributedProperties = ImmutableHashSet<string>.Empty;
        private readonly object syncObject = new object();

        public bool IsDistributedProperty(string key)
        {
            return distributedProperties.Contains(key);
        }

        public void AddDistributedProperty(string key)
        {
            lock (syncObject)
            {
                distributedProperties = distributedProperties.Add(key);
            }
        }
    }
}