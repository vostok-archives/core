using System;
using System.Globalization;

namespace Vostok.Commons.Utilities
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan Multiply(this TimeSpan ts, double multiplier)
        {
            return TimeSpan.FromTicks((long) (ts.Ticks*multiplier));
        }

        public static TimeSpan Divide(this TimeSpan ts, double divisor)
        {
            return TimeSpan.FromTicks((long) (ts.Ticks/divisor));
        }

        public static TimeSpan Min(TimeSpan ts1, TimeSpan ts2)
        {
            return ts1 <= ts2 ? ts1 : ts2;
        }

        public static TimeSpan Max(TimeSpan ts1, TimeSpan ts2)
        {
            return ts1 >= ts2 ? ts1 : ts2;
        }

        public static string ToPrettyString(this TimeSpan time)
        {
            if (time.TotalDays >= 1)
                return time.TotalDays.ToString("0.###", CultureInfo.InvariantCulture) + " days";

            if (time.TotalHours >= 1)
                return time.TotalHours.ToString("0.###", CultureInfo.InvariantCulture) + " hours";

            if (time.TotalMinutes >= 1)
                return time.TotalMinutes.ToString("0.###", CultureInfo.InvariantCulture) + " minutes";

            if (time.TotalSeconds >= 1)
                return time.TotalSeconds.ToString("0.###", CultureInfo.InvariantCulture) + " seconds";

            if (time.TotalMilliseconds >= 1)
                return time.TotalMilliseconds.ToString("0.###", CultureInfo.InvariantCulture) + " milliseconds";

            return time.ToString();
        }
    }
}
