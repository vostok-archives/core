using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Airlock.Client.Tests
{
    [TestFixture]
    internal class MemoryManager_Tests
    {
        private const int size = 10 * 1024;

        [Test]
        public void TryCreateBuffer_should_return_false_when_maxMemory_less_than_initBufferSize()
        {
            var memoryManager = new MemoryManager(size - 1, size);

            memoryManager.TryCreateBuffer(out _).Should().BeFalse();
        }

        [Test]
        public void TryCreateBuffer_should_return_true_when_there_is_available_memory_and_false_when_opposite()
        {
            var memoryManager = new MemoryManager(size * 2, size);

            memoryManager.TryCreateBuffer(out _).Should().BeTrue();
            memoryManager.TryCreateBuffer(out _).Should().BeTrue();
            memoryManager.TryCreateBuffer(out _).Should().BeFalse();
        }

        [Test]
        public void TryCreateBuffer_should_return_buffers_of_correct_initial_size()
        {
            var memoryManager = new MemoryManager(size * 2, size);

            memoryManager.TryCreateBuffer(out var buffer);

            buffer.Should().NotBeNull();
            buffer.Should().HaveCount(size);
        }

        [Test]
        public void TryReserve_should_return_true_when_there_is_available_memory_and_false_when_opposite()
        {
            var memoryManager = new MemoryManager(size * 2, size);

            memoryManager.TryReserveBytes(size).Should().BeTrue();
            memoryManager.TryReserveBytes(size).Should().BeTrue();
            memoryManager.TryReserveBytes(size).Should().BeFalse();
        }

        [Test]
        public void TryReserve_should_reserve_max_amount_size_when_it_is_greater_than_buffers_grow_in()
        {
            var memoryManager = new MemoryManager(size * 2, size);

            memoryManager.TryReserveBytes(size * 3).Should().BeFalse();
        }
    }
}