using System.Collections.Generic;
using System.Threading;

namespace Vostok.Flow
{
    internal class ContextConfiguration : IContextConfiguration
    {
        private readonly object syncObject = new object();
        private HashSet<string> distributedProperties = new HashSet<string>();

        public bool IsDistributedProperty(string key)
        {
            return distributedProperties.Contains(key);
        }

        public void AddDistributedProperty(string key)
        {
            lock (syncObject)
            {
                if (distributedProperties.Contains(key))
                    return;

                var newProperties = new HashSet<string>(distributedProperties)
                {
                    key
                };

                Interlocked.Exchange(ref distributedProperties, newProperties);
            }
        }
    }
}
