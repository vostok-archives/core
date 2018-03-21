using System;
using System.Text;

namespace Vostok.Commons.Binary
{
    public interface IBinaryWriter
    {
        int Position { get; set; }

        void Write(int value);
        void Write(long value);
        void Write(short value);

        void Write(uint value);
        void Write(ulong value);
        void Write(ushort value);

        void Write(byte value);
        void Write(bool value);
        void Write(float value);
        void Write(double value);
        void Write(Guid value);

        void Write(string value, Encoding encoding);
        void WriteWithoutLengthPrefix(string value, Encoding encoding);

        void Write(byte[] value);
        void Write(byte[] value, int offset, int length);
        void WriteWithoutLengthPrefix(byte[] value);
        void WriteWithoutLengthPrefix(byte[] value, int offset, int length);
    }
}
