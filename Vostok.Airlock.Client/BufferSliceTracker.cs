using System.Collections.Generic;
using System.Linq;
using Vostok.Commons.Utilities;

namespace Vostok.Airlock
{
    internal class BufferSliceTracker
    {
        private readonly Dictionary<IBuffer, List<BufferSlice>> slicesByBuffer = new Dictionary<IBuffer, List<BufferSlice>>(ReferenceEqualityComparer<IBuffer>.Instance);

        public void Reset()
        {
            slicesByBuffer.Clear();
        }

        public bool TryCompleteSnapshot(BufferSlice slice)
        {
            if (IsCompleteBuffer(slice))
                return true;

            var buffer = slice.Buffer;

            if (!slicesByBuffer.TryGetValue(buffer, out var slices))
                slicesByBuffer[buffer] = slices = new List<BufferSlice>();

            slices.Add(slice);

            if (IsCompleteBuffer(buffer, slices))
            {
                slicesByBuffer.Remove(buffer);
                return true;
            }

            return false;
        }

        private static bool IsCompleteBuffer(BufferSlice slice)
        {
            return slice.Length == slice.Buffer.SnapshotLength;
        }

        private static bool IsCompleteBuffer(IBuffer buffer, IList<BufferSlice> slices)
        {
            return slices.Sum(s => s.Length) == buffer.SnapshotLength;
        }
    }
}
