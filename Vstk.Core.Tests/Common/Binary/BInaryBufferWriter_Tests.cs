using System;
using FluentAssertions;
using NUnit.Framework;
using Vstk.Commons.Binary;

namespace Vstk.Common.Binary
{
    [TestFixture]
    internal class BinaryBufferWriter_Tests
    {
        [Test]
        public void Should_grow_twice_as_large_when_exceeding_capacity()
        {
            var writer = new BinaryBufferWriter(20);

            writer.WriteWithoutLengthPrefix(new byte[21]);

            writer.Buffer.Should().HaveCount(40);
        }

        [Test]
        public void Should_grow_multiple_times_if_needed()
        {
            var writer = new BinaryBufferWriter(1);

            writer.Write(Guid.NewGuid());

            writer.Buffer.Should().HaveCount(16);
        }

        [Test]
        public void Length_should_catch_up_with_position_after_using_setter()
        {
            var writer = new BinaryBufferWriter(16);

            writer.Write(1);

            writer.Position += 5;

            writer.Length.Should().Be(writer.Position);
        }

        [Test]
        public void Length_should_catch_up_with_position_after_using_seek_method()
        {
            var writer = new BinaryBufferWriter(16);

            writer.Write(1);

            writer.Seek(5);

            writer.Length.Should().Be(writer.Position);
        }

        [Test]
        public void Position_property_setter_should_check_array_lower_bound()
        {
            var writer = new BinaryBufferWriter(16);

            Action action = () => writer.Position = -1;

            action.ShouldThrow<IndexOutOfRangeException>();
        }

        [Test]
        public void Position_property_setter_should_check_array_upper_bound()
        {
            var writer = new BinaryBufferWriter(16);

            Action action = () => writer.Position = 17;

            action.ShouldThrow<IndexOutOfRangeException>();
        }

        [Test]
        public void Resize_performed_right_after_manual_position_change_should_not_lose_recent_data()
        {
            var writer = new BinaryBufferWriter(8);

            writer.Write(1);

            BitConverter.GetBytes(2).CopyTo(writer.Buffer, 4);

            writer.Position += 4;

            writer.EnsureCapacity(100);

            writer.Position.Should().Be(8);
            writer.Length.Should().Be(8);

            var reader = new BinaryBufferReader(writer.Buffer, 0);

            reader.ReadInt32().Should().Be(1);
            reader.ReadInt32().Should().Be(2);
        }
    }
}