using System;
using System.IO;
using System.Text;

namespace Vostok.Airlock
{
    internal partial class Buffer
    {
        public void Write(int value)
        {
            EnsureAvailableBytes(sizeof(int));

            binaryWriter.Write(value);
        }

        public void Write(long value)
        {
            EnsureAvailableBytes(sizeof(long));

            binaryWriter.Write(value);
        }

        public void Write(short value)
        {
            EnsureAvailableBytes(sizeof(short));

            binaryWriter.Write(value);
        }

        public void Write(uint value)
        {
            EnsureAvailableBytes(sizeof(uint));

            binaryWriter.Write(value);
        }

        public void Write(ulong value)
        {
            EnsureAvailableBytes(sizeof(ulong));

            binaryWriter.Write(value);
        }

        public void Write(ushort value)
        {
            EnsureAvailableBytes(sizeof(ushort));

            binaryWriter.Write(value);
        }

        public void Write(byte value)
        {
            EnsureAvailableBytes(sizeof(byte));

            binaryWriter.Write(value);
        }

        public void Write(bool value)
        {
            EnsureAvailableBytes(sizeof(byte));

            binaryWriter.Write(value);
        }

        public void Write(float value)
        {
            EnsureAvailableBytes(sizeof(float));

            binaryWriter.Write(value);
        }

        public void Write(double value)
        {
            EnsureAvailableBytes(sizeof(double));

            binaryWriter.Write(value);
        }

        public unsafe void Write(Guid value)
        {
            EnsureAvailableBytes(sizeof(Guid));

            binaryWriter.Write(value);
        }

        public void Write(string value, Encoding encoding)
        {
            EnsureAvailableBytes(sizeof(int) + encoding.GetMaxByteCount(value.Length));

            binaryWriter.Write(value, encoding);
        }

        public void WriteWithoutLengthPrefix(string value, Encoding encoding)
        {
            EnsureAvailableBytes(encoding.GetMaxByteCount(value.Length));

            binaryWriter.WriteWithoutLengthPrefix(value, encoding);
        }

        public void Write(byte[] value)
        {
            EnsureAvailableBytes(sizeof(int) + value.Length);

            binaryWriter.Write(value);
        }

        public void Write(byte[] value, int offset, int length)
        {
            EnsureAvailableBytes(sizeof(int) + length);

            binaryWriter.Write(value, offset, length);
        }

        public void WriteWithoutLengthPrefix(byte[] value)
        {
            EnsureAvailableBytes(value.Length);

            binaryWriter.WriteWithoutLengthPrefix(value);
        }

        public void WriteWithoutLengthPrefix(byte[] value, int offset, int length)
        {
            EnsureAvailableBytes(length);

            binaryWriter.WriteWithoutLengthPrefix(value, offset, length);
        }

        private void EnsureAvailableBytes(int amount)
        {
            var currentLength = binaryWriter.Buffer.Length;
            var expectedLength = binaryWriter.Position + amount;
            if (expectedLength <= currentLength)
                return;

            var remainingBytes = currentLength - Position;
            var reserveAmount = Math.Max(currentLength, amount - remainingBytes);

            if (!memoryManager.TryReserveBytes(reserveAmount))
                throw new InternalBufferOverflowException();
        }
    }
}