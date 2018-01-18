using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Commons.Binary;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Logging.Logs;

namespace Vostok.Airlock.Client.Tests
{
    [TestFixture]
    internal class BufferSliceFactory_Tests
    {
        private Buffer buffer;
        private BufferSliceFactory factory;
        private RecordSerializer recordSerializer;
        private StringSerializer itemSerializer;
        private int totalLength;

        [SetUp]
        public void TestSetup()
        {
            buffer = new Buffer(new BinaryBufferWriter(64), new MemoryManager(long.MaxValue, 64));
            factory = new BufferSliceFactory();

            totalLength = 0;
            recordSerializer = new RecordSerializer(1.Terabytes(), new ConsoleLog());
            itemSerializer = new StringSerializer();
        }

        [Test]
        public void Should_return_empty_buffer_as_single_slice()
        {
            var slice = factory.Cut(buffer, 32).Should().ContainSingle().Which;

            slice.Buffer.Should().BeSameAs(buffer);
            slice.Offset.Should().Be(0);
            slice.Length.Should().Be(0);
            slice.Items.Should().Be(0);
        }

        [Test]
        public void Should_return_single_slice_for_a_buffer_that_entirely_fits_into_max_slice_size()
        {
            totalLength += WriteRecord("message1");
            totalLength += WriteRecord("message2");
            totalLength += WriteRecord("message3");

            buffer.MakeSnapshot();

            factory.Cut(buffer, totalLength)
                .Should()
                .ContainSingle()
                .Which
                .Should()
                .Be(new BufferSlice(buffer, 0, buffer.SnapshotLength, buffer.SnapshotCount));
        }

        [Test]
        public void Should_fail_when_a_single_record_is_larger_than_maximum_slice_size()
        {
            totalLength += WriteRecord("message");

            buffer.MakeSnapshot();

            Action action = () => factory.Cut(buffer, 5).Count();

            Console.Out.WriteLine(action.ShouldThrow<Exception>().Which);
        }

        [Test]
        public void Should_correctly_split_buffer_into_slices_when_records_fit_exactly_into_slices()
        {
            totalLength += WriteRecord("m1");
            totalLength += WriteRecord("m2");
            totalLength += WriteRecord("m3");
            totalLength += WriteRecord("m4");
            totalLength += WriteRecord("m5");
            totalLength += WriteRecord("m6");

            buffer.MakeSnapshot();

            var slices = factory.Cut(buffer, totalLength/3);

            slices.Should()
                .Equal(
                    new BufferSlice(buffer, 0*totalLength/3, totalLength/3, 2),
                    new BufferSlice(buffer, 1*totalLength/3, totalLength/3, 2),
                    new BufferSlice(buffer, 2*totalLength/3, totalLength/3, 2)
                );
        }

        [Test]
        public void Should_correctly_split_buffer_into_slices_when_records_leave_a_bit_of_space_in_each_slice()
        {
            totalLength += WriteRecord("m1");
            totalLength += WriteRecord("m2");
            totalLength += WriteRecord("m3");
            totalLength += WriteRecord("m4");
            totalLength += WriteRecord("m5");
            totalLength += WriteRecord("m6");

            buffer.MakeSnapshot();

            var slices = factory.Cut(buffer, totalLength/3 + 2);

            slices.Should()
                .Equal(
                    new BufferSlice(buffer, 0*totalLength/3, totalLength/3, 2),
                    new BufferSlice(buffer, 1*totalLength/3, totalLength/3, 2),
                    new BufferSlice(buffer, 2*totalLength/3, totalLength/3, 2)
                );
        }

        [Test]
        public void Should_correctly_split_buffer_into_slices_when_records_are_too_large_to_fit_two_in_one_slice()
        {
            totalLength += WriteRecord("m1");
            totalLength += WriteRecord("m2");
            totalLength += WriteRecord("m3");
            totalLength += WriteRecord("m4");
            totalLength += WriteRecord("m5");
            totalLength += WriteRecord("m6");

            buffer.MakeSnapshot();

            var slices = factory.Cut(buffer, totalLength/3 - 1).ToArray();

            slices.Should().HaveCount(6);

            var singleRecordLength = totalLength/6;

            for (var i = 0; i < 6; i++)
            {
                slices[i].Offset.Should().Be(i*singleRecordLength);
                slices[i].Length.Should().Be(singleRecordLength);
                slices[i].Items.Should().Be(1);
            }
        }

        [Test]
        public void Should_correctly_split_buffer_into_slices_when_records_are_of_uneven_size()
        {
            var r1 = WriteRecord("--"); // fits into slice 1
            var r2 = WriteRecord("---"); // fits into slice 1
            var r3 = WriteRecord("----"); // fits into slice 2
            var r4 = WriteRecord("--"); // fits into slice 2
            var r5 = WriteRecord("-------"); // occupies slice 3
            var r6 = WriteRecord("---"); // remainder for slice 4

            buffer.MakeSnapshot();

            var slices = factory.Cut(buffer, r1 + r2 + 2);

            slices.Should()
                .Equal(
                    new BufferSlice(buffer, 0, r1 + r2, 2),
                    new BufferSlice(buffer, r1 + r2, r3 + r4, 2),
                    new BufferSlice(buffer, r1 + r2 + r3 + r4, r5, 1),
                    new BufferSlice(buffer, r1 + r2 + r3 + r4 + r5, r6, 1)
                );
        }

        [Test]
        public void Should_only_consider_snapshot_portion_of_the_buffer()
        {
            var r1 = WriteRecord("--");         // fits into slice 1
            var r2 = WriteRecord("---");        // fits into slice 1
            var r3 = WriteRecord("----");       // fits into slice 2

            buffer.MakeSnapshot();

            WriteRecord("--");         // outside of snapshot
            WriteRecord("-------");    // outside of snapshot
            WriteRecord("---");        // outside of snapshot

            var slices = factory.Cut(buffer, r1 + r2 + 2);

            slices.Should().Equal(
                new BufferSlice(buffer, 0, r1 + r2, 2),
                new BufferSlice(buffer, r1 + r2, r3, 1)
            );
        }

        private int WriteRecord(string message)
        {
            var positionBefore = buffer.Position;

            recordSerializer.TrySerialize(message, itemSerializer, DateTimeOffset.UtcNow, buffer);

            buffer.WrittenRecords++;

            return buffer.Position - positionBefore;
        }

        private class StringSerializer : IAirlockSerializer<string>
        {
            public void Serialize(string item, IAirlockSink sink)
            {
                sink.Writer.Write(item);
            }
        }
    }
}
