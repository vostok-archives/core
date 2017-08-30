using System;
using System.Text;

namespace Vostok.Commons.Binary
{
    public interface IBinaryReader
    {
        long Position { get; set; }

        int ReadInt32();
        long ReadInt64();
        short ReadInt16();

        uint ReadUInt32();
        ulong ReadUInt64();
        ushort ReadUInt16();

        Guid ReadGuid();
        byte ReadByte();
        bool ReadBool();
        float ReadFloat();
        double ReadDouble();

        string ReadString(Encoding encoding);
        string ReadString(Encoding encoding, int length);

        byte[] ReadByteArray();
        byte[] ReadByteArray(int length);
    }
}
