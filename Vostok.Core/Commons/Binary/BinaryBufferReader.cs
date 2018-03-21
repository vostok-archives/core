using System;
using System.Text;

namespace Vostok.Commons.Binary
{
    public class BinaryBufferReader : IBinaryReader
    {
        public BinaryBufferReader(byte[] buffer, int position)
        {
            Buffer = buffer;
            Position = position;
        }

        public byte[] Buffer { get; }

        public int Position { get; set; }

        public int BytesRemaining => Buffer.Length - Position;

        public unsafe int ReadInt32()
        {
            CheckBounds(sizeof(int));

            int result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(int*)ptr;
            Position += sizeof(int);

            return result;
        }

        public unsafe long ReadInt64()
        {
            CheckBounds(sizeof(long));

            long result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(long*)ptr;
            Position += sizeof(long);

            return result;
        }

        public unsafe short ReadInt16()
        {
            CheckBounds(sizeof(short));

            short result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(short*)ptr;
            Position += sizeof(short);

            return result;
        }

        public unsafe uint ReadUInt32()
        {
            CheckBounds(sizeof(uint));

            uint result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(uint*)ptr;
            Position += sizeof(uint);

            return result;
        }

        public unsafe ulong ReadUInt64()
        {
            CheckBounds(sizeof(ulong));

            ulong result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(ulong*)ptr;
            Position += sizeof(ulong);

            return result;
        }

        public unsafe ushort ReadUInt16()
        {
            CheckBounds(sizeof(ushort));

            ushort result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(ushort*)ptr;
            Position += sizeof(ushort);

            return result;
        }

        public unsafe Guid ReadGuid()
        {
            CheckBounds(sizeof(Guid));

            Guid result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(Guid*)ptr;
            Position += sizeof(Guid);

            return result;
        }

        public byte ReadByte()
        {
            return Buffer[Position++];
        }

        public bool ReadBool()
        {
            return ReadByte() != 0;
        }

        public unsafe float ReadFloat()
        {
            CheckBounds(sizeof(float));

            float result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(float*)ptr;
            Position += sizeof(float);

            return result;
        }

        public unsafe double ReadDouble()
        {
            CheckBounds(sizeof(double));

            double result;

            fixed (byte* ptr = &Buffer[Position])
                result = *(double*)ptr;
            Position += sizeof(double);

            return result;
        }

        public string ReadString(Encoding encoding)
        {
            var size = ReadInt32();

            CheckBounds(size);

            var result = encoding.GetString(Buffer, Position, size);
            Position += size;

            return result;
        }

        public string ReadString(Encoding encoding, int size)
        {
            CheckBounds(size);

            var result = encoding.GetString(Buffer, Position, size);
            Position += size;

            return result;
        }

        public byte[] ReadByteArray()
        {
            var size = ReadInt32();

            CheckBounds(size);

            var result = new byte[size];

            System.Buffer.BlockCopy(Buffer, Position, result, 0, size);
            Position += size;

            return result;
        }

        public byte[] ReadByteArray(int size)
        {
            CheckBounds(size);

            var result = new byte[size];

            System.Buffer.BlockCopy(Buffer, Position, result, 0, size);
            Position += size;

            return result;
        }

        private void CheckBounds(int size)
        {
            if (Position + size > Buffer.Length)
                throw new IndexOutOfRangeException();
        }
    }
}