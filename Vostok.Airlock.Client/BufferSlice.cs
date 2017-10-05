namespace Vostok.Airlock
{
    internal struct BufferSlice
    {
        public BufferSlice(IBuffer buffer, int offset, int length)
        {
            Buffer = buffer;
            Offset = offset;
            Length = length;
        }

        public readonly IBuffer Buffer;
        public readonly int Offset;
        public readonly int Length;
    }
}