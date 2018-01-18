using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Commons.Utilities;

namespace Vostok.Airlock.Client.Tests
{
    [TestFixture]
    internal class BufferSliceTracker_Tests
    {
        private readonly BufferSliceTracker tracker;

        private IBuffer buffer1;
        private IBuffer buffer2;
        private IBuffer buffer3;

        public BufferSliceTracker_Tests()
        {
            tracker = new BufferSliceTracker();
        }

        [SetUp]
        public void TestSetup()
        {
            buffer1 = Substitute.For<IBuffer>();
            buffer2 = Substitute.For<IBuffer>();
            buffer3 = Substitute.For<IBuffer>();

            buffer1.SnapshotLength.Returns(10);
            buffer2.SnapshotLength.Returns(20);
            buffer3.SnapshotLength.Returns(30);

            tracker.Reset();
        }

        [Test]
        public void TryCompleteSnapshot_should_return_true_for_a_whole_buffer_slice()
        {
            tracker.TryCompleteSnapshot(CreateSlice(buffer1, buffer1.SnapshotLength)).Should().BeTrue();
            tracker.TryCompleteSnapshot(CreateSlice(buffer2, buffer2.SnapshotLength)).Should().BeTrue();
            tracker.TryCompleteSnapshot(CreateSlice(buffer3, buffer3.SnapshotLength)).Should().BeTrue();
        }

        [Test]
        public void TryCompleteSnapshot_should_return_false_for_a_partial_buffer_slice_which_does_not_complete_buffer_length()
        {
            tracker.TryCompleteSnapshot(CreateSlice(buffer1, buffer1.SnapshotLength - 1)).Should().BeFalse();
            tracker.TryCompleteSnapshot(CreateSlice(buffer2, buffer2.SnapshotLength - 1)).Should().BeFalse();
            tracker.TryCompleteSnapshot(CreateSlice(buffer3, buffer3.SnapshotLength - 1)).Should().BeFalse();
        }

        [Test]
        public void TryCompleteSnapshot_should_return_true_for_a_partial_buffer_slice_which_completes_buffer_length()
        {
            tracker.TryCompleteSnapshot(CreateSlice(buffer1, buffer1.SnapshotLength - 1));
            tracker.TryCompleteSnapshot(CreateSlice(buffer2, buffer2.SnapshotLength - 1));
            tracker.TryCompleteSnapshot(CreateSlice(buffer3, buffer3.SnapshotLength - 1));

            tracker.TryCompleteSnapshot(CreateSlice(buffer1, 1)).Should().BeTrue();
            tracker.TryCompleteSnapshot(CreateSlice(buffer2, 1)).Should().BeTrue();
            tracker.TryCompleteSnapshot(CreateSlice(buffer3, 1)).Should().BeTrue();
        }

        private static BufferSlice CreateSlice(IBuffer buffer, int length)
        {
            return new BufferSlice(buffer, ThreadSafeRandom.Next(), length, ThreadSafeRandom.Next());
        }
    }
}
