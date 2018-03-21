using System;
using System.Text;

namespace Vostok.Commons.Binary
{
    public class BinaryBufferWriter : IBinaryWriter
    {
        private byte[] buffer;
        private int offset;
        private int length;

        public BinaryBufferWriter(byte[] buffer)
        {
            this.buffer = buffer;

            Reset();
        }

        public BinaryBufferWriter(int initialCapacity)
        {
            Reset(initialCapacity);
        }

        public int Position
        {
            get => offset;
            set
            {
                if (value < 0 || value > buffer.Length)
                    throw new IndexOutOfRangeException();

                offset = value;

                if (offset > length)
                    length = offset;
            }
        }

        public byte[] Buffer => buffer;

        public int Length => length;

        public ArraySegment<byte> FilledSegment => new ArraySegment<byte>(buffer, 0, length);

        public void Reset(int neededCapacity = 0)
        {
            if (buffer == null || buffer.Length < neededCapacity)
                buffer = new byte[neededCapacity];

            offset = 0;
            length = 0;
        }

        public void EnsureCapacity(int neededBytes)
        {
            var remainingBytes = buffer.Length - offset;
            if (remainingBytes >= neededBytes)
                return;

            var newCapacity = buffer.Length + Math.Max(neededBytes - remainingBytes, buffer.Length);
            var newBuffer = new byte[newCapacity];

            System.Buffer.BlockCopy(buffer, 0, newBuffer, 0, length);

            buffer = newBuffer;
        }

        public void Seek(int position)
        {
            Position = position;
        }

        public void SeekToStart()
        {
            Seek(0);
        }

        public void SeekToEnd()
        {
            Seek(length);
        }

        public unsafe void Write(int value)
        {
            EnsureCapacity(sizeof(int));

            fixed (byte* ptr = &buffer[offset])
                *(int*)ptr = value;

            offset += sizeof(int);

            if (offset > length)
                length = offset;
        }

        public unsafe void Write(long value)
        {
            EnsureCapacity(sizeof(long));

            fixed (byte* ptr = &buffer[offset])
                *(long*)ptr = value;
            offset += sizeof(long);

            if (offset > length)
                length = offset;
        }

        public unsafe void Write(short value)
        {
            EnsureCapacity(sizeof(short));

            fixed (byte* ptr = &buffer[offset])
                *(short*)ptr = value;
            offset += sizeof(short);

            if (offset > length)
                length = offset;
        }

        public unsafe void Write(uint value)
        {
            EnsureCapacity(sizeof(uint));

            fixed (byte* ptr = &buffer[offset])
                *(uint*)ptr = value;
            offset += sizeof(uint);

            if (offset > length)
                length = offset;
        }

        public unsafe void Write(ulong value)
        {
            EnsureCapacity(sizeof(ulong));

            fixed (byte* ptr = &buffer[offset])
                *(ulong*)ptr = value;
            offset += sizeof(ulong);

            if (offset > length)
                length = offset;
        }

        public unsafe void Write(ushort value)
        {
            EnsureCapacity(sizeof(ushort));

            fixed (byte* ptr = &buffer[offset])
                *(ushort*)ptr = value;
            offset += sizeof(ushort);

            if (offset > length)
                length = offset;
        }

        public void Write(byte value)
        {
            EnsureCapacity(1);

            buffer[offset++] = value;

            if (offset > length)
                length = offset;
        }

        public void Write(bool value)
        {
            Write(value ? (byte) 1 : (byte) 0);
        }

        public unsafe void Write(float value)
        {
            EnsureCapacity(sizeof(float));

            fixed (byte* ptr = &buffer[offset])
                *(float*)ptr = value;
            offset += sizeof(float);

            if (offset > length)
                length = offset;
        }

        public unsafe void Write(double value)
        {
            EnsureCapacity(sizeof(double));

            fixed (byte* ptr = &buffer[offset])
                *(double*)ptr = value;
            offset += sizeof(double);

            if (offset > length)
                length = offset;
        }

        public unsafe void Write(Guid value)
        {
            EnsureCapacity(sizeof(Guid));

            fixed (byte* ptr = &buffer[offset])
                *(Guid*)ptr = value;
            offset += sizeof(Guid);

            if (offset > length)
                length = offset;
        }

        public void Write(string value, Encoding encoding)
        {
            EnsureCapacity(encoding.GetMaxByteCount(value.Length) + sizeof(int));

            var byteCount = encoding.GetBytes(value, 0, value.Length, buffer, offset + sizeof(int));
            Write(byteCount);
            offset += byteCount;

            if (offset > length)
                length = offset;
        }

        public void WriteWithoutLengthPrefix(string value, Encoding encoding)
        {
            EnsureCapacity(encoding.GetMaxByteCount(value.Length));

            offset += encoding.GetBytes(value, 0, value.Length, buffer, offset);

            if (offset > length)
                length = offset;
        }

        public void Write(byte[] value)
        {
            Write(value, 0, value.Length);
        }

        public void Write(byte[] value, int off, int len)
        {
            EnsureCapacity(len + sizeof(int));

            Write(len);
            System.Buffer.BlockCopy(value, off, buffer, offset, len);
            offset += len;

            if (offset > length)
                length = offset;
        }

        public void WriteWithoutLengthPrefix(byte[] value)
        {
            WriteWithoutLengthPrefix(value, 0, value.Length);
        }

        public void WriteWithoutLengthPrefix(byte[] value, int off, int len)
        {
            EnsureCapacity(len);

            System.Buffer.BlockCopy(value, off, buffer, offset, len);
            offset += len;

            if (offset > length)
                length = offset;
        }
    }
}
