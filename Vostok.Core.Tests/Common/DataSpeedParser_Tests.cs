using NUnit.Framework;
using Vstk.Commons.Model;
using Vstk.Commons.Utilities;

namespace Vstk.Common
{
    [TestFixture]
    internal class DataSpeedParser_Tests
    {
        [TestCase("45,1 PB/sec", 45.1)]
        [TestCase("45,1 PB/second", 45.1)]
        [TestCase("45,1 petabytes/sec.", 45.1)]
        [TestCase("45,1 petabytes/second.", 45.1)]
        public void Should_parse_given_petabytes_per_second_representation(string input, double expected)
        {
            Assert.AreEqual(DataSpeed.FromPetabytesPerSecond(expected), DataSpeedParser.Parse(input));
            Assert.AreEqual(DataSpeed.FromPetabytesPerSecond(expected), DataSpeedParser.Parse(input.ToLower()));
            Assert.AreEqual(DataSpeed.FromPetabytesPerSecond(expected), DataSpeedParser.Parse(input.ToUpper()));
        }

        [TestCase("45,1 TB/sec", 45.1)]
        [TestCase("45,1 TB/second", 45.1)]
        [TestCase("45,1 terabytes/sec.", 45.1)]
        [TestCase("45,1 terabytes/second.", 45.1)]
        public void Should_parse_given_terabytes_per_second_representation(string input, double expected)
        {
            Assert.AreEqual(DataSpeed.FromTerabytesPerSecond(expected), DataSpeedParser.Parse(input));
            Assert.AreEqual(DataSpeed.FromTerabytesPerSecond(expected), DataSpeedParser.Parse(input.ToLower()));
            Assert.AreEqual(DataSpeed.FromTerabytesPerSecond(expected), DataSpeedParser.Parse(input.ToUpper()));
        }

        [TestCase("45.1 GB/sec", 45.1)]
        [TestCase("45.1 GB/second", 45.1)]
        [TestCase("45.1 gigabytes/sec.", 45.1)]
        [TestCase("45.1 gigabytes/second.", 45.1)]
        public void Should_parse_given_gigabytes_per_second_representation(string input, double expected)
        {
            Assert.AreEqual(DataSpeed.FromGigabytesPerSecond(expected), DataSpeedParser.Parse(input));
            Assert.AreEqual(DataSpeed.FromGigabytesPerSecond(expected), DataSpeedParser.Parse(input.ToLower()));
            Assert.AreEqual(DataSpeed.FromGigabytesPerSecond(expected), DataSpeedParser.Parse(input.ToUpper()));
        }

        [TestCase("45,1 MB/sec", 45.1)]
        [TestCase("45,1 MB/second", 45.1)]
        [TestCase("45,1 megabytes/sec.", 45.1)]
        [TestCase("45,1 megabytes/second.", 45.1)]
        public void Should_parse_given_megabytes_per_second_representation(string input, double expected)
        {
            Assert.AreEqual(DataSpeed.FromMegabytesPerSecond(expected), DataSpeedParser.Parse(input));
            Assert.AreEqual(DataSpeed.FromMegabytesPerSecond(expected), DataSpeedParser.Parse(input.ToLower()));
            Assert.AreEqual(DataSpeed.FromMegabytesPerSecond(expected), DataSpeedParser.Parse(input.ToUpper()));
        }

        [TestCase("45.1 KB/sec", 45.1)]
        [TestCase("45.1 KB/second", 45.1)]
        [TestCase("45.1 kilobytes/sec.", 45.1)]
        [TestCase("45.1 kilobytes/second.", 45.1)]
        public void Should_parse_given_kilobytes_per_second_representation(string input, double expected)
        {
            Assert.AreEqual(DataSpeed.FromKilobytesPerSecond(expected), DataSpeedParser.Parse(input));
            Assert.AreEqual(DataSpeed.FromKilobytesPerSecond(expected), DataSpeedParser.Parse(input.ToLower()));
            Assert.AreEqual(DataSpeed.FromKilobytesPerSecond(expected), DataSpeedParser.Parse(input.ToUpper()));
        }

        [TestCase("45 B/sec", 45)]
        [TestCase("45 B/second", 45)]
        [TestCase("45 bytes/sec.", 45)]
        [TestCase("45 bytes/second.", 45)]
        public void Should_parse_given_bytes_per_second_representation(string input, long expected)
        {
            Assert.AreEqual(DataSpeed.FromBytesPerSecond(expected), DataSpeedParser.Parse(input));
            Assert.AreEqual(DataSpeed.FromBytesPerSecond(expected), DataSpeedParser.Parse(input.ToLower()));
            Assert.AreEqual(DataSpeed.FromBytesPerSecond(expected), DataSpeedParser.Parse(input.ToUpper()));
        }
    }
}