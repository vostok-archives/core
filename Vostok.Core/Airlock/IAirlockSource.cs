using System.IO;
using Vostok.Commons.Binary;

namespace Vostok.Airlock
{
    public interface IAirlockSource
    {
        Stream ReadStream { get; }

        IBinaryReader Reader { get; }
    }
}