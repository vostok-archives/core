using System;
using System.Text;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.ToCore.Utilities
{
    public static class StringHelpers
    {
        public static bool IgnoreCaseEquals(this string value, string other)
        {
            return string.Equals(value, other, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IgnoreCaseStartsWith(this string value, string prefix)
        {
            return value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }

        public static string[] SplitOnFirst(this string value, char separator)
        {
            if (value == null)
                return new string[0];
            var pos = value.IndexOf(separator);
            return pos == -1
                ? new[] { value }
                : new[] { value.Substring(0, pos), value.Substring(pos + 1) };
        }

        public static string[] SplitOnFirst(this string value, string separator)
        {
            if (value == null)
                return new string[0];
            var pos = value.IndexOf(separator);
            return pos == -1
                ? new[] { value }
                : new[] { value.Substring(0, pos), value.Substring(pos + 1) };
        }

        public static string[] SplitOnLast(this string value, char separator)
        {
            if (value == null)
                return new string[0];
            var pos = value.LastIndexOf(separator);
            return pos == -1
                ? new[] { value }
                : new[] { value.Substring(0, pos), value.Substring(pos + 1) };
        }

        public static string[] SplitOnLast(this string value, string separator)
        {
            if (value == null)
                return new string[0];
            var pos = value.LastIndexOf(separator);
            return pos == -1
                ? new[] { value }
                : new[] { value.Substring(0, pos), value.Substring(pos + 1) };
        }

        public static string CapitalizeWords(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var result = new StringBuilder(value);
            for (var i = 0; i < result.Length; i++)
            {
                var current = result[i];
                if (!char.IsWhiteSpace(current) && (i == 0 || char.IsWhiteSpace(result[i - 1])))
                {
                    result[i] = char.ToUpper(current);
                }
            }
            return result.ToString();
        }

        public static string ObtainEndsWithWhiteSpace(this string value, char obtainWith = ' ')
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var lastSymbol = value[value.Length - 1];
            return char.IsWhiteSpace(lastSymbol) ? value : value + obtainWith;
        }
    }
}