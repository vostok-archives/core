using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Vostok.Logging.Logs
{
    public class LogEventFormatter_Tests
    {
        [Theory]
        [MemberData(nameof(FormatMessage_Cases))]
        public void FormatMessage(string template, object[] parameters, string expected)
        {
            LogEventFormatter.FormatMessage(template, parameters).Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(FormatProperties_Cases))]
        public void FormatProperties(IReadOnlyDictionary<string, object> properties, string expected)
        {
            LogEventFormatter.FormatProperties(properties).Should().Be(expected);
        }

        public static IEnumerable<object[]> FormatMessage_Cases()
        {
            yield return new object[] {string.Empty, Array.Empty<object>(), string.Empty};
            yield return new object[] {"Hello", Array.Empty<object>(), "Hello"};
            yield return new object[] {"Hello {Name}", new object[] {"James"}, "Hello James"};
            yield return new object[] {"Hello {0}", new object[] {"James"}, "Hello James"};
            yield return new object[] {"Hello {Name} {Surname}", new object[] {"James", "Bond"}, "Hello James Bond"};
            yield return new object[] {"Hello {0} {1}", new object[] {"James", "Bond"}, "Hello James Bond"};
            yield return new object[] {"{Surname}. James {Surname}", new object[] {"Bond"}, "Bond. James Bond"};
            yield return new object[] {"{0}. James {0}", new object[] {"Bond"}, "Bond. James Bond"};
        }

        public static IEnumerable<object[]> FormatProperties_Cases()
        {
            yield return new object[] {new Dictionary<string, object>(), "{}"};
            yield return new object[] {new Dictionary<string, object> {{"Key", "Value"}}, "{Key: Value}"};
            yield return new object[] {new Dictionary<string, object> {{"Key", "Value"}, {"AnotherKey", "Value"}}, "{Key: Value, AnotherKey: Value}"};
        }
    }
}