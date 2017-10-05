namespace Vostok.Airlock
{
    internal struct BufferSlice
    {
        public BufferSlice(IBuffer buffer, int offset, int length, int count)
        {
            Buffer = buffer;
            Offset = offset;
            Length = length;
            Count = count;
        }

        public readonly IBuffer Buffer;
        public readonly int Offset;
        public readonly int Length;
        public readonly int Count;
    }
}