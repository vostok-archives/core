using System.Linq;
using System.Text;

namespace Vostok.Airlock
{
    public static class RoutingKey
    {
        public const string Separator = ".";
        public const string AppEventsSuffix = "app-events";
        public const string TraceEventsSuffix = "trace-events";
        public const string MetricsSuffix = "metrics";
        public const string TracesSuffix = "traces";
        public const string LogsSuffix = "logs";

        private const char UnacceptableCharPlaceholder = '-';

        public static string CreatePrefix(string project, string environment, string service)
        {
            return Create(project, environment, service);
        }

        public static string Create(params string[] parts)
        {
            return string.Join(Separator, parts.Select(FixInvalidChars));
        }

        public static string AddSuffix(string prefix, string suffix)
        {
            return prefix + Separator + FixInvalidChars(suffix);
        }

        private static string FixInvalidChars(string s)
        {
            if (s.All(IsAcceptableChar))
                return s;
            var sb = new StringBuilder(s);
            for (var i = 0; i < sb.Length; i++)
            {
                if (IsAcceptableChar(sb[i]))
                    continue;
                if (IsAcceptableUpperCaseChar(sb[i]))
                    sb[i] = char.ToLower(sb[i]);
                else
                    sb[i] = UnacceptableCharPlaceholder;
            }
            return sb.ToString();
        }

        private static bool IsAcceptableChar(char c)
        {
            return c >= 'a' && c <= 'z'
                   || c >= 0 && c <= '9'
                   || c == UnacceptableCharPlaceholder;
        }

        private static bool IsAcceptableUpperCaseChar(char c)
        {
            return c >= 'A' && c <= 'Z';
        }
    }
}