using NUnit.Framework;
using Vstk.Commons.Utilities;

namespace Vstk.Common
{
    [TestFixture]
    internal class DoubleParser_Tests
    {
        [TestCase("0", 0D)]
        [TestCase("1", 1D)]
        [TestCase("0.345", 0.345D)]
        [TestCase("0,345", 0.345D)]
        public void Should_parse_correct_value_from_given_input(string input, double expected)
        {
            Assert.AreEqual(expected, DoubleParser.ParseDouble(input));
        }
    }
}