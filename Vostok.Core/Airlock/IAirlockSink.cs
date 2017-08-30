using System.IO;
using Vostok.Commons.Binary;

namespace Vostok.Airlock
{
    public interface IAirlockSink
    {
        Stream WriteStream { get; }

        IBinaryWriter Writer { get; }
    }
}
