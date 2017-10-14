namespace Vostok.Airlock
{
    internal struct BufferSlice
    {
        public BufferSlice(IBuffer buffer, int offset, int length, int items)
        {
            Buffer = buffer;
            Offset = offset;
            Length = length;
            Items = items;
        }

        public readonly IBuffer Buffer;
        public readonly int Offset;
        public readonly int Length;
        public readonly int Items;
    }
}