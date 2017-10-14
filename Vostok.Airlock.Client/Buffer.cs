using System;
using System.IO;
using System.Text;
using Vostok.Commons.Binary;

namespace Vostok.Airlock
{
    internal class Buffer : IBuffer, IBinaryWriter
    {
        private readonly BinaryBufferWriter binaryWriter;
        private readonly IMemoryManager memoryManager;
        private AirlockWriteStream writeStream;

        public Buffer(BinaryBufferWriter binaryWriter, IMemoryManager memoryManager)
        {
            this.binaryWriter = binaryWriter;
            this.memoryManager = memoryManager;
        }

        public int WrittenRecords { get; set; }

        public int Position
        {
            get => binaryWriter.Position;
            set => binaryWriter.Position = value;
        }

        public int SnapshotLength { get; private set; }

        public int SnapshotCount { get; private set; }

        public bool SnapshotIsGarbage { get; private set; }

        public byte[] InternalBuffer => binaryWriter.Buffer;

        public IBinaryWriter Writer => this;

        public Stream WriteStream => writeStream ?? (writeStream = new AirlockWriteStream(this));

        public void MakeSnapshot()
        {
            CollectGarbage();

            SnapshotLength = Position;
            SnapshotCount = WrittenRecords;
            SnapshotIsGarbage = false;
        }

        public void DiscardSnapshot()
        {
            SnapshotIsGarbage = true;
        }

        public void CollectGarbage()
        {
            if (!SnapshotIsGarbage)
                return;

            if (SnapshotLength > 0)
            {
                if (SnapshotLength != Position)
                {
                    var bytesWrittenAfterSnapshot = Position - SnapshotLength;
                    System.Buffer.BlockCopy(InternalBuffer, SnapshotLength, InternalBuffer, 0, bytesWrittenAfterSnapshot);
                    Position = bytesWrittenAfterSnapshot;
                }
                else
                {
                    binaryWriter.Reset();
                }
                SnapshotLength = 0;
            }

            if (SnapshotCount > 0)
            {
                WrittenRecords -= SnapshotCount;
                SnapshotCount = 0;
            }

            SnapshotIsGarbage = false;
        }

        #region IBinaryWriter implementation

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

        #endregion
    }
}
