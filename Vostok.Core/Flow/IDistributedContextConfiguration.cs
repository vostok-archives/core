using System;
using System.Collections.Generic;

namespace Vostok.Flow
{
    public interface IDistributedContextConfiguration
    {
        IDictionary<string, Type> Properties { get; }
        ISerializer Serializer { get; set; }
        IDecoder KeyDecoder { get; set; }
    }
}
