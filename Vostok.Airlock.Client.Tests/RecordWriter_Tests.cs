using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Vostok.Airlock.Client.Tests
{
    [TestFixture]
    internal class RecordWriter_Tests
    {
        private IBufferPool bufferPool;
        private IBuffer buffer;
        private IRecordSerializer recordSerializer;
        private RecordWriter recordWriter;

        private string item;
        private DateTimeOffset timestamp;
        private IAirlockSerializer<string> itemSerializer;

        [SetUp]
        public void TestSetup()
        {
            timestamp = DateTimeOffset.UtcNow;
            item = Guid.NewGuid().ToString();
            itemSerializer = Substitute.For<IAirlockSerializer<string>>();

            buffer = Substitute.For<IBuffer>();
            buffer.WrittenRecords.Returns(5);
            buffer.Position.Returns(10);

            bufferPool = Substitute.For<IBufferPool>();
            bufferPool
                .TryAcquire(out _)
                .Returns(
                    x =>
                    {
                        x[0] = buffer;
                        return buffer != null;
                    });

            recordSerializer = Substitute.For<IRecordSerializer>();
            recordSerializer
                .TrySerialize(item, itemSerializer, timestamp, buffer)
                .Returns(true);

            recordWriter = new RecordWriter(recordSerializer);
        }

        [Test]
        public void TryWrite_should_return_false_when_buffer_cannot_be_acquired()
        {
            buffer = null;

            TryWrite().Should().BeFalse();
        }

        [Test]
        public void TryWrite_should_return_false_when_record_serialization_fails()
        {
            recordSerializer
                .TrySerialize(item, itemSerializer, timestamp, buffer)
                .Returns(false);

            TryWrite().Should().BeFalse();
        }

        [Test]
        public void TryWrite_should_rollback_buffer_position_when_record_serialization_fails()
        {
            recordSerializer
                .TrySerialize(item, itemSerializer, timestamp, buffer)
                .Returns(false);

            TryWrite();

            buffer.Received().Position = 10;
        }

        [Test]
        public void TryWrite_should_release_buffer_to_pool_when_record_serialization_fails()
        {
            recordSerializer
                .TrySerialize(item, itemSerializer, timestamp, buffer)
                .Returns(false);

            TryWrite();

            bufferPool.Received(1).Release(buffer);
        }

        [Test]
        public void TryWrite_should_return_true_when_record_serialization_succeeds()
        {
            TryWrite().Should().BeTrue();
        }

        [Test]
        public void TryWrite_should_release_buffer_to_pool_when_record_serialization_succeeds()
        {
            TryWrite();

            bufferPool.Received(1).Release(buffer);
        }

        [Test]
        public void TryWrite_should_increment_written_records_on_buffer_when_record_serialization_succeeds()
        {
            TryWrite();

            buffer.Received().WrittenRecords = 6;
        }

        private bool TryWrite()
        {
            return recordWriter.TryWrite(item, itemSerializer, timestamp, bufferPool);
        }
    }
}
