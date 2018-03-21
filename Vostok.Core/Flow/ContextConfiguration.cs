using System;
using System.Collections.Generic;
using Vstk.Commons.Collections;

namespace Vstk.Flow
{
    internal class ContextConfiguration : IContextConfiguration
    {
        public ISet<string> DistributedProperties { get; } = new ConcurrentSet<string>(StringComparer.Ordinal);
    }
}
