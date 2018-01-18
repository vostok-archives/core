using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Vostok.Airlock.Client.Tests
{
    [TestFixture]
    internal class BufferPool_Tests
    {
        private IMemoryManager memoryManager;
        private BufferPool pool;

        [SetUp]
        public void TestSetup()
        {
            memoryManager = Substitute.For<IMemoryManager>();
            memoryManager.TryReserveBytes(Arg.Any<int>()).Returns(true);
            memoryManager.TryCreateBuffer(out _)
                .Returns(
                    x =>
                    {
                        x[0] = new byte[4];
                        return true;
                    });

            pool = new BufferPool(memoryManager, 3);
        }

        [Test]
        public void Should_preallocate_initial_buffers_in_ctor()
        {
            memoryManager.Received(3).TryCreateBuffer(out _);
        }

        [Test]
        public void TryAcquire_should_not_allocate_new_buffers_when_there_are_spare_ones()
        {
            memoryManager.ClearReceivedCalls();

            for (var i = 0; i < 3; i++)
                pool.TryAcquire(out _).Should().BeTrue();

            memoryManager.ReceivedCalls().Should().BeEmpty();
        }

        [Test]
        public void TryAcquire_should_allocate_new_buffers_when_there_are_no_spare_ones()
        {
            memoryManager.ClearReceivedCalls();

            for (var i = 0; i < 5; i++)
                pool.TryAcquire(out _).Should().BeTrue();

            memoryManager.Received(2).TryCreateBuffer(out _);
        }

        [Test]
        public void TryAcquire_should_collect_garbage_in_buffers()
        {
            pool = new BufferPool(memoryManager, 1);

            pool.TryAcquire(out var buffer);

            buffer.Writer.Write(int.MaxValue);
            buffer.Writer.Write(int.MaxValue);
            buffer.WrittenRecords = 2;
            buffer.MakeSnapshot();
            buffer.DiscardSnapshot();
            buffer.Writer.Write(int.MaxValue);
            buffer.WrittenRecords++;

            pool.Release(buffer);
            pool.TryAcquire(out buffer);

            buffer.Position.Should().Be(4);
            buffer.WrittenRecords.Should().Be(1);
        }

        [Test]
        public void Should_cycle_between_allocated_buffers_in_fifo_order()
        {
            pool.TryAcquire(out var buffer1);
            pool.TryAcquire(out var buffer2);
            pool.TryAcquire(out var buffer3);

            pool.Release(buffer1);
            pool.Release(buffer2);
            pool.Release(buffer3);

            pool.TryAcquire(out var buffer).Should().BeTrue();
            buffer.Should().BeSameAs(buffer1);
            pool.Release(buffer);

            pool.TryAcquire(out buffer).Should().BeTrue();
            buffer.Should().BeSameAs(buffer2);
            pool.Release(buffer);

            pool.TryAcquire(out buffer).Should().BeTrue();
            buffer.Should().BeSameAs(buffer3);
            pool.Release(buffer);
        }

        [Test]
        public void GetSnapshot_should_only_return_non_empty_buffers()
        {
            pool.TryAcquire(out var buffer1);
            pool.TryAcquire(out var buffer2);
            pool.TryAcquire(out var buffer3);

            pool.Release(buffer1);
            pool.Release(buffer2);
            pool.Release(buffer3);

            buffer1.Writer.Write(int.MaxValue);
            buffer3.Writer.Write(int.MaxValue);

            pool.GetSnapshot().Should().Equal(buffer1, buffer3);
        }

        [Test]
        public void GetSnapshot_should_snapshot_returned_buffers()
        {
            pool.TryAcquire(out var buffer1);
            pool.TryAcquire(out var buffer2);
            pool.TryAcquire(out var buffer3);

            pool.Release(buffer1);
            pool.Release(buffer2);
            pool.Release(buffer3);

            buffer1.Writer.Write(int.MaxValue);
            buffer3.Writer.Write(int.MaxValue);

            pool.GetSnapshot();

            buffer1.SnapshotLength.Should().Be(4);
            buffer3.SnapshotLength.Should().Be(4);
        }

        [Test]
        public void GetSnapshot_should_produce_repeatable_results()
        {
            pool.TryAcquire(out var buffer1);
            pool.TryAcquire(out var buffer2);
            pool.TryAcquire(out var buffer3);

            pool.Release(buffer1);
            pool.Release(buffer2);
            pool.Release(buffer3);

            buffer1.Writer.Write(int.MaxValue);
            buffer3.Writer.Write(int.MaxValue);

            var snapshot1 = pool.GetSnapshot();
            var snapshot2 = pool.GetSnapshot();

            snapshot2.Should().Equal(snapshot1);
        }

        [Test]
        public void GetSnapshot_should_not_make_buffers_unavailable_for_consumers()
        {
            pool.TryAcquire(out var buffer1);
            pool.TryAcquire(out var buffer2);
            pool.TryAcquire(out var buffer3);

            pool.Release(buffer1);
            pool.Release(buffer2);
            pool.Release(buffer3);

            buffer1.Writer.Write(int.MaxValue);
            buffer3.Writer.Write(int.MaxValue);

            pool.GetSnapshot();

            pool.TryAcquire(out var acuiredBuffer1);
            pool.TryAcquire(out var acuiredBuffer2);
            pool.TryAcquire(out var acuiredBuffer3);

            acuiredBuffer1.Should().BeSameAs(buffer1);
            acuiredBuffer2.Should().BeSameAs(buffer2);
            acuiredBuffer3.Should().BeSameAs(buffer3);
        }
    }
}