using System;
using NUnit.Framework;
using Vstk.Commons.Model;
using Vstk.Commons.Utilities;

namespace Vstk.Common
{
    [TestFixture]
    internal class DataSizeParser_Tests
    {
        [TestCase("45,1 Petabytes", 45.1)]
        [TestCase("45,1 petabytes", 45.1)]
        [TestCase("45,1 PB", 45.1)]
        [TestCase("45,1 PB.", 45.1)]
        [TestCase("45,1PB", 45.1)]
        [TestCase("45,1PB.", 45.1)]
        [TestCase("0PB.", 0)]
        public void Should_parse_given_petabytes_representation(string input, double expected)
        {
            Assert.AreEqual(DataSize.FromPetabytes(expected), DataSizeParser.Parse(input));
            Assert.AreEqual(DataSize.FromPetabytes(expected), DataSizeParser.Parse(input.ToLower()));
            Assert.AreEqual(DataSize.FromPetabytes(expected), DataSizeParser.Parse(input.ToUpper()));
            Assert.AreEqual(DataSize.FromPetabytes(expected), DataSizeParser.Parse(DataSize.FromPetabytes(expected).ToString()));
        }

        [TestCase("45,1 Terabytes", 45.1)]
        [TestCase("45,1 terabytes", 45.1)]
        [TestCase("45,1 TB", 45.1)]
        [TestCase("45,1 TB.", 45.1)]
        [TestCase("45,1TB", 45.1)]
        [TestCase("45,1TB.", 45.1)]
        [TestCase("0TB.", 0)]
        public void Should_parse_given_terabytes_representation(string input, double expected)
        {
            Assert.AreEqual(DataSize.FromTerabytes(expected), DataSizeParser.Parse(input));
            Assert.AreEqual(DataSize.FromTerabytes(expected), DataSizeParser.Parse(input.ToLower()));
            Assert.AreEqual(DataSize.FromTerabytes(expected), DataSizeParser.Parse(input.ToUpper()));
            Assert.AreEqual(DataSize.FromTerabytes(expected), DataSizeParser.Parse(DataSize.FromTerabytes(expected).ToString()));
        }

        [TestCase("45,1 Gigabytes", 45.1)]
        [TestCase("45,1 gigabytes", 45.1)]
        [TestCase("45,1 GB", 45.1)]
        [TestCase("45,1 GB.", 45.1)]
        [TestCase("45,1GB", 45.1)]
        [TestCase("45,1GB.", 45.1)]
        [TestCase("0GB.", 0)]
        public void Should_parse_given_gigabytes_representation(string input, double expected)
        {
            Assert.AreEqual(DataSize.FromGigabytes(expected), DataSizeParser.Parse(input));
            Assert.AreEqual(DataSize.FromGigabytes(expected), DataSizeParser.Parse(input.ToLower()));
            Assert.AreEqual(DataSize.FromGigabytes(expected), DataSizeParser.Parse(input.ToUpper()));
            Assert.AreEqual(DataSize.FromGigabytes(expected), DataSizeParser.Parse(DataSize.FromGigabytes(expected).ToString()));
        }

        [TestCase("45.1 Megabytes", 45.1)]
        [TestCase("45.1 megabytes", 45.1)]
        [TestCase("45.1 MB", 45.1)]
        [TestCase("45.1 MB.", 45.1)]
        [TestCase("45.1MB", 45.1)]
        [TestCase("45.1MB.", 45.1)]
        [TestCase("0MB.", 0)]
        public void Should_parse_given_megabytes_representation(string input, double expected)
        {
            Assert.AreEqual(DataSize.FromMegabytes(expected), DataSizeParser.Parse(input));
            Assert.AreEqual(DataSize.FromMegabytes(expected), DataSizeParser.Parse(input.ToLower()));
            Assert.AreEqual(DataSize.FromMegabytes(expected), DataSizeParser.Parse(input.ToUpper()));
            Assert.AreEqual(DataSize.FromMegabytes(expected), DataSizeParser.Parse(DataSize.FromMegabytes(expected).ToString()));
        }

        [TestCase("128 Kilobytes", 128)]
        [TestCase("128 kilobytes", 128)]
        [TestCase("128 KB", 128)]
        [TestCase("128 KB.", 128)]
        [TestCase("128KB", 128)]
        [TestCase("128KB.", 128)]
        [TestCase("0KB.", 0)]
        public void Should_parse_given_kilobytes_representation(string input, double expected)
        {
            Console.Out.WriteLine(DataSize.FromKilobytes(expected));

            Assert.AreEqual(DataSize.FromKilobytes(expected), DataSizeParser.Parse(input));
            Assert.AreEqual(DataSize.FromKilobytes(expected), DataSizeParser.Parse(input.ToLower()));
            Assert.AreEqual(DataSize.FromKilobytes(expected), DataSizeParser.Parse(input.ToUpper()));
            Assert.AreEqual(DataSize.FromKilobytes(expected), DataSizeParser.Parse(DataSize.FromKilobytes(expected).ToString()));
        }

        [TestCase("45 Bytes", 45)]
        [TestCase("45 bytes", 45)]
        [TestCase("45 B", 45)]
        [TestCase("45 b.", 45)]
        [TestCase("45B", 45)]
        [TestCase("45B.", 45)]
        [TestCase("0B.", 0)]
        public void Should_parse_given_bytes_representation(string input, long expected)
        {
            Assert.AreEqual(DataSize.FromBytes(expected), DataSizeParser.Parse(input));
            Assert.AreEqual(DataSize.FromBytes(expected), DataSizeParser.Parse(input.ToLower()));
            Assert.AreEqual(DataSize.FromBytes(expected), DataSizeParser.Parse(input.ToUpper()));
            Assert.AreEqual(DataSize.FromBytes(expected), DataSizeParser.Parse(DataSize.FromBytes(expected).ToString()));
        }

        [Test]
        public void Should_parse_representation_without_unit()
        {
            Assert.AreEqual(DataSize.FromBytes(5345), DataSizeParser.Parse("5345 "));
        }
    }
}