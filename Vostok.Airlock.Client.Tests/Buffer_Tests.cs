using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Commons.Binary;

namespace Vostok.Airlock.Client.Tests
{
    [TestFixture]
    internal class Buffer_Tests
    {
        private IMemoryManager memoryManager;
        private BinaryBufferWriter underlyingWriter;
        private Buffer buffer;

        [SetUp]
        public void TestSetup()
        {
            memoryManager = Substitute.For<IMemoryManager>();
            memoryManager.TryReserveBytes(Arg.Any<int>()).Returns(true);
            underlyingWriter = new BinaryBufferWriter(4);
            buffer = new Buffer(underlyingWriter, memoryManager);
        }

        [Test]
        public void Should_correctly_reserve_memory_when_buffer_grows_twice_as_large()
        {
            buffer.Write(1);
            buffer.Write(2);
            buffer.Position.Should().Be(8);

            memoryManager.Received(1).TryReserveBytes(4);
        }

        [Test]
        public void Should_correctly_reserve_memory_when_buffer_grows_more_than_twice_as_large()
        {
            buffer.Write(Guid.NewGuid());
            buffer.Position.Should().Be(16);

            memoryManager.Received(1).TryReserveBytes(12);
        }

        [Test]
        public void Should_correctly_write_short_values()
        {
            var value = (short) -5345;

            buffer.Write(value);
            buffer.Position.Should().Be(2);

            CreateReader().ReadInt16().Should().Be(value);
        }

        [Test]
        public void Should_correctly_write_ushort_values()
        {
            var value = (ushort) 45325;

            buffer.Write(value);
            buffer.Position.Should().Be(2);

            CreateReader().ReadUInt16().Should().Be(value);
        }

        [Test]
        public void Should_correctly_write_int_values()
        {
            var value = -556;

            buffer.Write(value);
            buffer.Position.Should().Be(4);

            CreateReader().ReadInt32().Should().Be(value);
        }

        [Test]
        public void Should_correctly_write_uint_values()
        {
            var value = 356344U;

            buffer.Write(value);
            buffer.Position.Should().Be(4);

            CreateReader().ReadUInt32().Should().Be(value);
        }

        [Test]
        public void Should_correctly_write_long_values()
        {
            var value = -54365346356L;

            buffer.Write(value);
            buffer.Position.Should().Be(8);

            CreateReader().ReadInt64().Should().Be(value);
        }

        [Test]
        public void Should_correctly_write_ulong_values()
        {
            var value = 54365346356UL;

            buffer.Write(value);
            buffer.Position.Should().Be(8);

            CreateReader().ReadUInt64().Should().Be(value);
        }

        [Test]
        public void Should_correctly_write_byte_values()
        {
            var value = (byte) 56;

            buffer.Write(value);
            buffer.Position.Should().Be(1);

            CreateReader().ReadByte().Should().Be(value);
        }

        [Test]
        public void Should_correctly_write_bool_values()
        {
            buffer.Write(true);
            buffer.Position.Should().Be(1);

            CreateReader().ReadBool().Should().Be(true);
        }

        [Test]
        public void Should_correctly_write_float_values()
        {
            var value = -534.535345345435f;

            buffer.Write(value);
            buffer.Position.Should().Be(4);

            CreateReader().ReadFloat().Should().Be(value);
        }

        [Test]
        public void Should_correctly_write_double_values()
        {
            var value = 65346534.5343;

            buffer.Write(value);
            buffer.Position.Should().Be(8);

            CreateReader().ReadDouble().Should().Be(value);
        }

        [Test]
        public void Should_correctly_write_guid_values()
        {
            var value = Guid.NewGuid();

            buffer.Write(value);
            buffer.Position.Should().Be(16);

            CreateReader().ReadGuid().Should().Be(value);
        }

        [Test]
        public void Should_correctly_write_string_values_with_length()
        {
            var value = Guid.NewGuid().ToString();

            buffer.Write(value);
            buffer.Position.Should().Be(40);

            CreateReader().ReadString().Should().Be(value);
        }

        [Test]
        public void Should_correctly_write_string_values_without_length()
        {
            var value = Guid.NewGuid().ToString();

            buffer.WriteWithoutLengthPrefix(value);
            buffer.Position.Should().Be(36);

            CreateReader().ReadString(36).Should().Be(value);
        }

        [Test]
        public void Should_correctly_write_byte_arrays_with_length()
        {
            var value = Guid.NewGuid().ToByteArray();

            buffer.Write(value);
            buffer.Position.Should().Be(20);

            CreateReader().ReadByteArray().Should().Equal(value);
        }

        [Test]
        public void Should_correctly_write_byte_arrays_without_length()
        {
            var value = Guid.NewGuid().ToByteArray();

            buffer.WriteWithoutLengthPrefix(value);
            buffer.Position.Should().Be(16);

            CreateReader().ReadByteArray(16).Should().Equal(value);
        }

        [Test]
        public void Should_correctly_write_byte_array_slices_with_length()
        {
            var value = Guid.NewGuid().ToByteArray();

            buffer.Write(value, 4, 10);
            buffer.Position.Should().Be(14);

            CreateReader().ReadByteArray().Should().Equal(value.Skip(4).Take(10));
        }

        [Test]
        public void Should_correctly_write_byte_array_slices_without_length()
        {
            var value = Guid.NewGuid().ToByteArray();

            buffer.WriteWithoutLengthPrefix(value, 4, 10);
            buffer.Position.Should().Be(10);

            CreateReader().ReadByteArray(10).Should().Equal(value.Skip(4).Take(10));
        }

        [Test]
        public void Should_fail_with_buffer_overflow_exception_when_memory_manager_disallows_allocation()
        {
            memoryManager.TryReserveBytes(Arg.Any<int>()).Returns(false);

            Action action = () => buffer.Write(int.MaxValue);

            action.ShouldNotThrow();
            action.ShouldThrowExactly<InternalBufferOverflowException>();
        }

        [Test]
        public void MakeSnapshot_should_capture_length()
        {
            buffer.Write(int.MaxValue);
            buffer.Write(int.MaxValue);

            buffer.MakeSnapshot();

            buffer.Write(int.MaxValue);

            buffer.SnapshotLength.Should().Be(8);
        }

        [Test]
        public void MakeSnapshot_should_capture_written_records_count()
        {
            buffer.WrittenRecords = 5;

            buffer.MakeSnapshot();

            buffer.WrittenRecords++;

            buffer.SnapshotCount.Should().Be(5);
        }

        [Test]
        public void MakeSnapshot_should_capture_underlying_writer_position_as_snapshot_length()
        {
            underlyingWriter.Write(int.MaxValue);
            underlyingWriter.Write(int.MaxValue);
            underlyingWriter.Position = 5;

            buffer.MakeSnapshot();

            buffer.SnapshotLength.Should().Be(5);
        }

        [Test]
        public void DiscardSnapshot_should_mark_snapshot_as_garbage()
        {
            buffer.Write(int.MaxValue);
            buffer.Write(int.MaxValue);

            buffer.MakeSnapshot();
            buffer.DiscardSnapshot();

            buffer.SnapshotIsGarbage.Should().BeTrue();
        }

        [Test]
        public void CollectGarbage_should_do_nothing_if_current_snapshot_is_not_garbage()
        {
            buffer.Write(int.MaxValue);
            buffer.Write(int.MaxValue);
            buffer.WrittenRecords = 2;

            buffer.MakeSnapshot();
            buffer.CollectGarbage();

            buffer.Position.Should().Be(8);
            buffer.SnapshotLength.Should().Be(8);
            buffer.SnapshotCount.Should().Be(2);
        }

        [Test]
        public void CollectGarbage_should_do_nothing_if_garbage_snapshot_is_empty()
        {
            buffer.MakeSnapshot();
            buffer.DiscardSnapshot();
            buffer.CollectGarbage();

            buffer.Position.Should().Be(0);
            buffer.SnapshotLength.Should().Be(0);
            buffer.SnapshotCount.Should().Be(0);
            buffer.SnapshotIsGarbage.Should().BeFalse();
        }

        [Test]
        public void CollectGarbage_should_correctly_drop_data_from_garbage_snapshot_when_there_is_appended_tail()
        {
            buffer.Write((byte) 1);
            buffer.Write((byte) 2);
            buffer.WrittenRecords = 2;

            buffer.MakeSnapshot();

            buffer.Write((byte) 3);
            buffer.WrittenRecords++;

            buffer.DiscardSnapshot();
            buffer.CollectGarbage();

            buffer.Position.Should().Be(1);
            buffer.WrittenRecords.Should().Be(1);
            buffer.SnapshotLength.Should().Be(0);
            buffer.SnapshotCount.Should().Be(0);
            // ReSharper disable once PossibleNullReferenceException
            underlyingWriter.FilledSegment.Array[underlyingWriter.FilledSegment.Offset].Should().Be(0x03);
        }

        [Test]
        public void CollectGarbage_should_correctly_drop_data_from_garbage_snapshot_when_there_is_no_appended_tail()
        {
            buffer.Write((byte)1);
            buffer.Write((byte)2);
            buffer.Write((byte)3);
            buffer.WrittenRecords = 3;

            buffer.MakeSnapshot();
            buffer.DiscardSnapshot();
            buffer.CollectGarbage();

            buffer.Position.Should().Be(0);
            buffer.WrittenRecords.Should().Be(0);
            buffer.SnapshotLength.Should().Be(0);
            buffer.SnapshotCount.Should().Be(0);
        }

        [Test]
        public void CollectGarbage_should_unmark_snapshot_as_garbag()
        {
            buffer.Write(int.MaxValue);

            buffer.MakeSnapshot();
            buffer.DiscardSnapshot();
            buffer.CollectGarbage();

            buffer.SnapshotIsGarbage.Should().BeFalse();
        }

        [Test]
        public void MakeSnapshot_should_collect_previous_garbage_and_unmark_new_snapshot_as_garbage_when_there_is_appended_data_tail()
        {
            buffer.Write((byte)1);
            buffer.Write((byte)2);
            buffer.WrittenRecords = 2;

            buffer.MakeSnapshot();

            buffer.Write((byte)3);
            buffer.WrittenRecords++;

            buffer.DiscardSnapshot();
            buffer.MakeSnapshot();

            buffer.Position.Should().Be(1);
            buffer.WrittenRecords.Should().Be(1);
            buffer.SnapshotLength.Should().Be(1);
            buffer.SnapshotCount.Should().Be(1);
            buffer.SnapshotIsGarbage.Should().BeFalse();
            // ReSharper disable once PossibleNullReferenceException
            underlyingWriter.FilledSegment.Array[underlyingWriter.FilledSegment.Offset].Should().Be(0x03);
        }

        [Test]
        public void MakeSnapshot_should_collect_previous_garbage_and_unmark_new_snapshot_as_garbage_when_there_is_no_appended_data_tail()
        {
            buffer.Write(int.MaxValue);
            buffer.Write(int.MaxValue);
            buffer.WrittenRecords = 2;

            buffer.MakeSnapshot();
            buffer.DiscardSnapshot();
            buffer.MakeSnapshot();

            buffer.Position.Should().Be(0);
            buffer.WrittenRecords.Should().Be(0);
            buffer.SnapshotLength.Should().Be(0);
            buffer.SnapshotCount.Should().Be(0);
            buffer.SnapshotIsGarbage.Should().BeFalse();
        }

        [Test]
        public void Should_expose_itself_as_sink_writer()
        {
            buffer.Writer.Should().BeSameAs(buffer);
        }

        [Test]
        public void Should_expose_an_airlock_write_stream_for_sink()
        {
            buffer.WriteStream.Should().BeOfType<AirlockWriteStream>();
        }

        [Test]
        public void Should_reuse_write_stream_for_sink()
        {
            var stream1 = buffer.WriteStream;
            var stream2 = buffer.WriteStream;

            stream2.Should().BeSameAs(stream1);
        }

        private IBinaryReader CreateReader()
        {
            return new BinaryBufferReader(underlyingWriter.Buffer, 0);
        }
    }
}