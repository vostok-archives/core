using FluentAssertions;
using NUnit.Framework;
using Vostok.Commons.Binary;

namespace Vostok.Airlock
{
    [TestFixture]
    internal class RequestMessageBuilder_Tests
    {
        private byte[] buffer;
        private RequestMessageBuilder builder;

        [SetUp]
        public void TestSetup()
        {
            buffer = new byte[128];
            builder = new RequestMessageBuilder(buffer);
        }

        [Test]
        public void TryAppend_should_return_true_when_group_fits_into_buffer()
        {
            builder.TryAppend("key", CreateGroupBuffer(new string('?', 30))).Should().BeTrue();
        }

        [Test]
        public void TryAppend_should_return_false_when_group_does_not_fit_into_buffer()
        {
            builder.TryAppend("key", CreateGroupBuffer(new string('?', 200))).Should().BeFalse();
        }

        [Test]
        public void TryAppend_should_correctly_assemble_message_from_multiple_record_groups()
        {
            builder.TryAppend("key1", CreateGroupBuffer("message1"));
            builder.TryAppend("key2", CreateGroupBuffer("message22"));
            builder.TryAppend("key3", CreateGroupBuffer("message333"));

            builder.Message.Count.Should().Be(81);

            var reader = new BinaryBufferReader(buffer, 0);

            reader.ReadInt16().Should().Be(1); // version
            reader.ReadInt32().Should().Be(3); // groups count

            reader.ReadString().Should().Be("key1");      // routing key
            reader.ReadInt32().Should().Be(1);           // payload count
            reader.ReadString().Should().Be("message1"); // payload message

            reader.ReadString().Should().Be("key2");      // routing key
            reader.ReadInt32().Should().Be(1);           // payload count
            reader.ReadString().Should().Be("message22"); // payload message

            reader.ReadString().Should().Be("key3");      // routing key
            reader.ReadInt32().Should().Be(1 );          // payload count
            reader.ReadString().Should().Be("message333"); // payload message
        }

        private static IBuffer CreateGroupBuffer(string content)
        {
            var groupBuffer = new Buffer(new BinaryBufferWriter(4), new MemoryManager(long.MaxValue, 4));

            groupBuffer.Write(content);
            groupBuffer.WrittenRecords++;
            groupBuffer.MakeSnapshot();
            groupBuffer.Write("garbage");

            return groupBuffer;
        }

    }
}