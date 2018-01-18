using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Commons.Binary;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Logging.Logs;

namespace Vostok.Airlock.Client.Tests
{
    [TestFixture]
    internal class RecordSerializer_Tests
    {
        private const int bufferSize = 32;
        private const int maxRecordSize = 40;

        private Buffer buffer;
        private MemoryManager manager;
        private RecordSerializer serializer;

        private DateTimeOffset timestamp;
        private string item;
        private IAirlockSerializer<string> itemSerializer;

        [SetUp]
        public void TestSetup()
        {
            manager = new MemoryManager(bufferSize*2, bufferSize);
            buffer = new Buffer(new BinaryBufferWriter(bufferSize), manager);
            serializer = new RecordSerializer(maxRecordSize.Bytes(), new ConsoleLog());

            timestamp = DateTimeOffset.Now;
            item = "Hello!";
            itemSerializer = Substitute.For<IAirlockSerializer<string>>();
            itemSerializer
                .WhenForAnyArgs(s => s.Serialize(null, null))
                .Do(info => info.Arg<IAirlockSink>().Writer.Write(info.Arg<string>()));
        }

        [Test]
        public void TrySerialize_should_correctly_serialize_record_given_sufficient_memory()
        {
            serializer.TrySerialize(item, itemSerializer, timestamp, buffer).Should().BeTrue();

            var reader = new BinaryBufferReader(buffer.InternalBuffer, 0);

            reader.ReadInt64().Should().Be(timestamp.ToUniversalTime().ToUnixTimeMilliseconds());
            reader.ReadInt32().Should().Be(10);
            reader.ReadString().Should().Be(item);
        }

        [Test]
        public void TrySerialize_should_return_false_when_available_memory_is_insufficient()
        {
            item = new string('!', bufferSize*4);

            serializer.TrySerialize(item, itemSerializer, timestamp, buffer).Should().BeFalse();
        }

        [Test]
        public void TrySerialize_should_return_false_when_record_size_exceeds_configured_limit()
        {
            item = new string('!', maxRecordSize + 1);

            serializer.TrySerialize(item, itemSerializer, timestamp, buffer).Should().BeFalse();
        }
    }
}
