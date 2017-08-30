using System;

namespace Vostok.Clusterclient.Helpers
{
    internal class TimeProvider : ITimeProvider
    {
        public DateTime GetCurrentTime()
        {
            return DateTime.UtcNow;
        }
    }
}
