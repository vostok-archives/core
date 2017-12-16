using System.Threading.Tasks;
using NUnit.Framework;
using Vostok.Commons.Synchronization;
using Vostok.Commons.Utilities;

namespace Vostok.Common.Synchronization
{
    public class AtomicDouble_Tests
    {
        [Test]
        public void AtomicDoubleMultithreadedAdd()
        {
            var threads = 1000;
            var diffs = new double[threads];
            double sum = 0;
            for (var i = 0; i < diffs.Length; i++)
            {
                var nextDouble = ThreadSafeRandom.NextDouble();
                diffs[i] = nextDouble;
                sum += nextDouble;
            }
            var counter = new AtomicDouble(0);
            Parallel.ForEach(diffs, x => counter.Add(x));
            Assert.AreEqual(sum, counter.Value, 0.0000000001);
        }

        [Test]
        public void AtomicIntMultithreadedAdd()
        {
            var threads = 1000;
            var diffs = new int[threads];
            var sum = 0;
            for (var i = 0; i < diffs.Length; i++)
            {
                var next = ThreadSafeRandom.Next(10000);
                diffs[i] = next;
                sum += next;
            }
            var counter = new AtomicDouble(0);
            Parallel.ForEach(diffs, x => counter.Add(x));
            Assert.AreEqual(sum, counter.Value);
        }
    }
}