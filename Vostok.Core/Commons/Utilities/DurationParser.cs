using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Vostok.Commons.Utilities
{
    public static class DurationParser
    {
        private static readonly Regex durationRegex =
            new Regex(
                @"^(?:(?<days>-?\d+\.?\d*)\s*(d|day|days))?\s*
                   (?:(?<hours>-?\d+\.?\d*)\s*(h|hour|hours))?\s*
                   (?:(?<minutes>-?\d+\.?\d*)\s*(m|min|minute|minutes))?\s*
                   (?:(?<seconds>-?\d+\.?\d*)\s*(s|sec|second|seconds))?\s*
                   (?:(?<millis>-?\d+\.?\d*)\s*(ms|msec|millisecond|milliseconds))?$",
                RegexOptions.Compiled|RegexOptions.CultureInvariant|RegexOptions.IgnoreCase|RegexOptions.IgnorePatternWhitespace);

        public static bool TryParse(string input, out TimeSpan duration)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                duration = default(TimeSpan);
                return false;
            }

            input = input.ToLowerInvariant().Replace(',', '.').Trim();
            if (TimeSpan.TryParse(input, CultureInfo.InvariantCulture, out duration))
                return true;

            var match = durationRegex.Match(input);
            if (!match.Success)
                return false;

            duration = TimeSpan.Zero;
            var success =
                TryAddGroup(match, "millis", TimeSpan.FromMilliseconds, ref duration)
                && TryAddGroup(match, "seconds", TimeSpan.FromSeconds, ref duration)
                && TryAddGroup(match, "minutes", TimeSpan.FromMinutes, ref duration)
                && TryAddGroup(match, "hours", TimeSpan.FromHours, ref duration)
                && TryAddGroup(match, "days", TimeSpan.FromDays, ref duration);

            return success;
        }

        private static bool TryAddGroup(Match match, string group, Func<double, TimeSpan> conversion, ref TimeSpan result)
        {
            if (!IsCaptured(match, group))
                return true;

            double groupValue;
            if (!TryParseDouble(match, group, out groupValue))
                return false;

            result = result + conversion(groupValue);
            return true;
        }

        private static bool IsCaptured(Match match, string group)
        {
            return match.Groups[group].Captures.Count > 0;
        }

        private static bool TryParseDouble(Match match, string group, out double result)
        {
            return double.TryParse(match.Groups[group].Captures[0].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }
    }
}