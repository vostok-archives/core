using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Vostok.Logging.Logs
{
    internal static class LogEventFormatter
    {
        private static readonly Regex Pattern = new Regex(@"(?<!{){@?(?<arg>[^ :{}]+)(?<format>:[^}]+)?}", RegexOptions.Compiled);

        public static string Format(LogEvent logEvent)
        {
            var message = FormatMessage(logEvent.MessageTemplate, logEvent.MessageParameters);
            var parameters = FormatProperties(logEvent.Properties);
            return $"{DateTime.Now:HH:mm:ss.fff} {logEvent.Level} {message} {logEvent.Exception}{Environment.NewLine}{parameters}{Environment.NewLine}";
        }

        internal static string FormatMessage(string template, object[] parameters)
        {
            if (parameters.Length == 0)
                return template;

            var processedArguments = new List<string>();

            foreach (Match match in Pattern.Matches(template))
            {
                var arg = match.Groups["arg"].Value;

                if (!int.TryParse(arg, out _))
                {
                    var argumentIndex = processedArguments.IndexOf(arg);
                    if (argumentIndex == -1)
                    {
                        argumentIndex = processedArguments.Count;
                        processedArguments.Add(arg);
                    }

                    template = ReplaceFirst(template, match.Value, "{" + argumentIndex + match.Groups["format"].Value + "}");
                }
            }

            return string.Format(CultureInfo.InvariantCulture, template, parameters);
        }

        internal static string FormatProperties(IReadOnlyDictionary<string, object> properties)
        {
            return "{" + string.Join(", ", properties.Select(x => $"{x.Key}: {x.Value}")) + "}";
        }

        private static string ReplaceFirst(string text, string search, string replace)
        {
            var position = text.IndexOf(search, StringComparison.Ordinal);
            if (position < 0)
                return text;
            return text.Substring(0, position) + replace + text.Substring(position + search.Length);
        }
    }
}