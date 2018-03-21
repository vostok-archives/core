using System.Collections.Generic;

namespace Vstk.Flow
{
    public interface IContextConfiguration
    {
        ISet<string> DistributedProperties { get; }
    }
}
