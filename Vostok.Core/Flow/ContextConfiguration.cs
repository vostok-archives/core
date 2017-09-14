using System;
using System.Collections.Generic;
using Vostok.Commons.Collections;

namespace Vostok.Flow
{
    internal class ContextConfiguration : IContextConfiguration
    {
        public ISet<string> DistributedProperties { get; } = new ConcurrentSet<string>(StringComparer.Ordinal);
    }
}
