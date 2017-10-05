using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Commons.Collections;

namespace Vostok.Common.Collections
{
    public class UnlimitedLazyPool_Tests
    {
        [Test]
        public void Acquire_should_return_new_object_when_pool_is_empty()
        {
            var newObject = new object();
            var pool = new UnlimitedLazyPool<object>(() => newObject);

            var actual = pool.Acquire();

            actual.Should().Be(newObject);
            pool.Available.Should().Be(0);
            pool.Allocated.Should().Be(1);
        }

        [Test]
        public void Release_should_return_in_pool()
        {
            var pool = new UnlimitedLazyPool<object>(() => new object());
            var poolObject = pool.Acquire();

            pool.Release(poolObject);

            pool.Allocated.Should().Be(1);
            pool.Available.Should().Be(1);
        }

        [Test]
        public void Acquire_should_return_object_from_pool_when_pool_is_not_empty()
        {
            var pool = new UnlimitedLazyPool<object>(() => new object());
            var poolObject = pool.Acquire();
            pool.Release(poolObject);

            var actual = pool.Acquire();

            actual.Should().Be(poolObject);
            pool.Available.Should().Be(0);
            pool.Allocated.Should().Be(1);
        }

        [Test]
        public void Dispose_should_clear_pool_when_pool_is_not_empty()
        {
            var pool = new UnlimitedLazyPool<object>(() => new object());
            var poolObject = pool.Acquire();
            pool.Release(poolObject);

            pool.Dispose();

            pool.Available.Should().Be(0);
            pool.Allocated.Should().Be(0);
        }

        [Test]
        public void Dispose_should_dispose_resources_when_resources_is_disposable()
        {
            var pool = new UnlimitedLazyPool<DisposableObject>(() => new DisposableObject());
            var object1 = pool.Acquire();
            object1.IsDisposed.Should().BeFalse();
            pool.Release(object1);

            var object2 = pool.Acquire();
            object2.IsDisposed.Should().BeFalse();
            pool.Release(object2);

            pool.Dispose();

            object1.IsDisposed.Should().BeTrue();
            object2.IsDisposed.Should().BeTrue();
        }

        [Test]
        public void Acquire_should_throw_exception_when_pool_is_disposed()
        {
            var pool = new UnlimitedLazyPool<object>(() => new object());

            pool.Dispose();

            Action acquire = () => pool.Acquire();

            acquire.ShouldThrow<ObjectDisposedException>();
        }

        private class DisposableObject : IDisposable
        {
            public bool IsDisposed { get; private set; } = false;

            public void Dispose()
            {
                IsDisposed = true;
            }
        }
    }
}