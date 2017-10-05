using System.Collections.Generic;
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

        public static string Create(string project, string environment, string service, params string[] suffix)
        {
            return AddSuffix(CreatePrefix(project, environment, service), suffix);
        }

        public static string CreatePrefix(string project, string environment, string service)
        {
            return Create(new[] {project, environment, service});
        }

        public static string AddSuffix(string prefix, params string[] suffix)
        {
            return prefix + Separator + Create(suffix);
        }

        private static string Create(IEnumerable<string> parts)
        {
            return string.Join(Separator, parts.Select(FixInvalidChars));
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