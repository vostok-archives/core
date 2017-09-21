using System.IO;
using Vostok.Commons.Binary;

namespace Vostok.Airlock
{
    public interface IAirlockDeserializationSink
    {
        Stream ReadStream { get; }

        IBinaryReader Reader { get; }
    }
}