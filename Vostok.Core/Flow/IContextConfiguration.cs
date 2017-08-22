using System.Collections.Generic;

namespace Vostok.Flow
{
    public interface IContextConfiguration
    {
        ISet<string> DistributedProperties { get; }
    }
}