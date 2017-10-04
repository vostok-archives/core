using System;
using System.Collections.Concurrent;

namespace Vostok.Airlock
{
    internal static class BufferQueueExtensions
    {
        public static void ForEachSafe(this ConcurrentQueue<IBuffer> self, Action<IBuffer> action)
        {
            var initialCount = self.Count;
            var firstElement = null as IBuffer;

            for (var i = 0; i < initialCount; i++)
            {
                if (!self.TryDequeue(out var buffer))
                    return;

                if (ReferenceEquals(firstElement, buffer))
                    return;

                if (firstElement == null)
                    firstElement = buffer;

                action(buffer);

                self.Enqueue(buffer);
            }
        }
    }
}
