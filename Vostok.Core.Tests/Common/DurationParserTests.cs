using System;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Commons.Utilities;

namespace Vostok.Common
{
    [TestFixture]
    public class DurationParserTests
    {
        
        private static readonly object[] correctCases = {
            new object[]{"1s", 1.Seconds()},
            new object[]{"2m120ms", 2.Minutes() + 120.Milliseconds() },
            new object[]{"10d1h2m30s456ms", 10.Days() + 1.Hours() + 2.Minutes() + 30.Seconds() + 456.Milliseconds()},
            new object[]{"1 day", 1.Days()},
            new object[]{"-3.4 min", TimeSpan.FromMinutes(-3.4)},
            new object[]{"2 min 30 sec 50 milliseconds", 2.Minutes() + 30.Seconds() + 50.Milliseconds()},
            new object[]{"20 m 30 ms", 20.Minutes() + 30.Milliseconds()},
            new object[]{1.Days().ToString("G", CultureInfo.InvariantCulture), 1.Days()},
            new object[]{"1 MINuTe", 1.Minutes()},
            new object[]{"1,5 sec", TimeSpan.FromSeconds(1.5)}
        };
        [Test, TestCaseSource(nameof(correctCases))]
        public void Should_parse_correctly(string input, TimeSpan expected)
        {
            TimeSpan result;

            var success = DurationParser.TryParse(input, out result);

            success.Should().BeTrue();
            result.Should().Be(expected);
        }

        private static readonly object[] failCases = {
            new object[]{"dsfsl", "not duration"},
            new object[]{"", "empty string"},
            new object[]{"10 minutes 1 day", "units should be in descending order"},
            new object[]{"0.3. day", "two dots in number"},
            new object[]{"0 ms 0 ms", "the same unit can appear only once"},
            new object[]{null, "null string"}
        };

        [Test, TestCaseSource(nameof(failCases))]
        public void Should_fail_if_input_is_not_a_correct_duration(string input, string reason)
        {
            var success = DurationParser.TryParse(input, out _);

            success.Should().BeFalse($"because {reason}. Input: [{input}]");
        }
    }
}