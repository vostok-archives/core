using System;
using Vostok.Commons.Binary;

namespace Vostok.Airlock
{
    internal partial class Buffer : IBuffer, IBinaryWriter
    {
        private readonly BinaryBufferWriter binaryWriter;
        private readonly IMemoryManager memoryManager;

        public Buffer(BinaryBufferWriter binaryWriter, IMemoryManager memoryManager)
        {
            this.binaryWriter = binaryWriter;
            this.memoryManager = memoryManager;
        }

        public Guid Id { get; } = Guid.NewGuid();

        public int WrittenRecords { get; set; }

        public int Position
        {
            get => binaryWriter.Position;
            set => binaryWriter.Position = value;
        }

        public byte[] InternalBuffer => binaryWriter.Buffer;
    }
}
