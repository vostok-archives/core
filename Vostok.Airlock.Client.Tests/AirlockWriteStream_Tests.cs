using FluentAssertions;
using NUnit.Framework;
using Vostok.Commons.Binary;

namespace Vostok.Airlock.Client.Tests
{
    [TestFixture]
    internal class AirlockWriteStream_Tests
    {
        private BinaryBufferWriter writer;
        private AirlockWriteStream stream;

        [SetUp]
        public void TestSetup()
        {
            writer = new BinaryBufferWriter(64);
            stream = new AirlockWriteStream(writer);
        }

        [Test]
        public void Write_methods_should_produce_correct_results()
        {
            stream.Write(new byte[] {1, 2}, 0, 2);
            stream.Write(new byte[] {1, 2, 3, 4, 5}, 2, 2);

            stream.WriteAsync(new byte[] {5, 6}, 0, 2);
            stream.WriteAsync(new byte[] {5, 6, 7, 8, 9}, 2, 2);

            stream.WriteByte(9);
            stream.WriteByte(10);

            writer.FilledSegment.Should().Equal(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        }

        [Test]
        public void Should_only_advertise_write_capabilities()
        {
            stream.CanWrite.Should().BeTrue();

            stream.CanRead.Should().BeFalse();
            stream.CanSeek.Should().BeFalse();
            stream.CanTimeout.Should().BeFalse();
        }
    }
}