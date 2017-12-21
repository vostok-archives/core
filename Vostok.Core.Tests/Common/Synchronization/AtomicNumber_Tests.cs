using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Vostok.Commons.Synchronization;
using Vostok.Commons.Utilities;

namespace Vostok.Common.Synchronization
{
    public class AtomicNumber_Tests
    {
        private const int threadCount = 1000;

        [Test]
        public void AtomicDoubleMultithreadedTest()
        {
            AtomicAddMultithreadedTest<double, AtomicDouble>(ThreadSafeRandom.NextDouble, x => x, (a, b) => a + b, (a, b) => Math.Abs(a - b) < 1e-10);
        }

        [Test]
        public void AtomicIntMultithreadedTest()
        {
            AtomicAddMultithreadedTest<int, AtomicInt>(ThreadSafeRandom.Next, x => x, (a, b) => a + b, (a, b) => a == b);
        }

        [Test]
        public void AtomicLongMultithreadedTest()
        {
            AtomicAddMultithreadedTest<long, AtomicLong>(() => ThreadSafeRandom.Next(0L, 100000L), x => x, (a, b) => a + b, (a, b) => a == b);
        }

        private void AtomicAddMultithreadedTest<T, TAtomic>(Func<T> getRandom, Func<int, T> fromInt, Func<T, T, T> add, Func<T, T, bool> areEqual)
            where TAtomic : IAtomicNumber<T>, new()
        {
            var counterIncrease = new TAtomic();
            var increaseToValues = new T[threadCount];
            var maxRandom = 0;
            for (var i = 0; i < increaseToValues.Length; i++)
            {
                var randomInt = ThreadSafeRandom.Next(100);
                maxRandom = Math.Max(maxRandom, randomInt);
                increaseToValues[i] = fromInt(randomInt);
            }
            Parallel.ForEach(increaseToValues, x => counterIncrease.TryIncreaseTo(x));
            Assert.True(areEqual(fromInt(maxRandom), counterIncrease.Value), $"{counterIncrease.Value} must be equal to {fromInt(maxRandom)} after multithreaded increase");

            for (var i = 0; i < increaseToValues.Length; i++)
            {
                increaseToValues[i] = fromInt(0);
            }
            var wasIncrease = new AtomicBoolean(false);
            Parallel.ForEach(increaseToValues, x =>
            {
                if (counterIncrease.TryIncreaseTo(fromInt(10)))
                {
                    Assert.True(wasIncrease.TrySetTrue());;
                }
            });

            var diffs = new T[threadCount];
            var sum = default(T);
            for (var i = 0; i < diffs.Length; i++)
            {
                var next = getRandom();
                diffs[i] = next;
                sum = add(sum, next);
            }
            var counter = new TAtomic();
            Parallel.ForEach(diffs, x => counter.Add(x));
            Assert.True(areEqual(sum, counter.Value), $"{counter.Value} must be equal to {sum} after multithreaded sum");

            var counterIncr = new TAtomic();
            Parallel.For(0, threadCount, x => counterIncr.Increment());
            Assert.True(areEqual(fromInt(threadCount), counterIncr.Value), $"{counterIncr.Value} must be equal to {fromInt(threadCount)} after multithreaded increment");

            var counterDecr = new TAtomic();
            Parallel.For(0, threadCount, x => counterDecr.Decrement());
            Assert.True(areEqual(fromInt(-threadCount), counterDecr.Value), $"{counterDecr.Value} must be equal to {fromInt(-threadCount)} after multithreaded decrement");
        }
    }
}