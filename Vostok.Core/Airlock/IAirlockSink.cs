using System.IO;

namespace Vostok.Airlock
{
    public interface IAirlockSink
    {
        Stream WriteStream { get; }

        void Write(int value);
        void Write(long value);
        void Write(byte value);
        void Write(byte[] value);
        void Write(string value);
        void Write(double value);
        // ...
    }
}