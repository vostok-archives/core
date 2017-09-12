using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Vostok.Flow
{
    internal class DistributedContextConfiguration : IDistributedContextConfiguration
    {
        public IDictionary<string, Type> Properties { get; } = new ConcurrentDictionary<string, Type>();
        public ISerializer Serializer { get; set; }
        public IDecoder KeyDecoder { get; set; }
    }
}
