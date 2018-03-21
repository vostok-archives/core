using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Vstk.Commons.Extensions.UnitConvertions;
using Vstk.Commons.Synchronization;
using Vstk.Logging;
using Vstk.Logging.Logs;

namespace Vstk.RetriableCall
{
    [TestFixture]
    public class RetriableCallStrategy_Tests
    {
        private const int expectedResult = 10;
        private readonly ConsoleLog log = new ConsoleLog();
        private readonly RetriableCallStrategy callStrategy = new RetriableCallStrategy(5, 500.Milliseconds(), 10.Seconds());
        private readonly Stopwatch stopwatch = new Stopwatch();

        [SetUp]
        public void Init()
        {
            stopwatch.Restart();
        }

        [TearDown]
        public void Done()
        {
            log.Info("elapsed = " + stopwatch.Elapsed);
        }

        [Test, TestCase(true), TestCase(false)]
        public void CallWithoutErrors_ReturnsWithowtDelays(bool async)
        {
            if (async)
                callStrategy.CallAsync(() => Task.CompletedTask, ex => true, log).GetAwaiter().GetResult();
            else
                callStrategy.Call(() => {}, ex => true, log);
            Assert.Less(stopwatch.ElapsedMilliseconds, 50, "ElapsedMilliseconds");
            log.Info("elapsed = " + stopwatch.Elapsed);
            stopwatch.Restart();
            var actualResult = async ? CallAsync(() => Task.FromResult(expectedResult), ex => true) : Call(() => expectedResult, ex => true);
            Assert.Less(stopwatch.ElapsedMilliseconds, 50, "ElapsedMilliseconds");
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test, TestCase(true), TestCase(false)]
        public void CallWithErrorsAndSuccessAtTheEnd_ReturnsSuccess(bool async)
        {
            var counter = new AtomicInt(0);
            int actualResult;
            actualResult = async ? CallAsync(() => TestFuncAsync(counter), ex => true) : Call(() => TestFunc(counter), ex => true);
            stopwatch.Stop();
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(3, counter.Value, "counter");
            Assert.Greater(stopwatch.ElapsedMilliseconds, 500*(1 + 1.7) - 100, "ElapsedMilliseconds");
            Assert.Less(stopwatch.ElapsedMilliseconds, 500*(1 + 2.5) + 1000, "ElapsedMilliseconds");
        }

        [Test, TestCase(true), TestCase(false)]
        public void CallWithRetriableErrors_ReturnsAggregateException(bool async)
        {
            var counter = new AtomicInt(0);
            if (async)
                Assert.Throws<AggregateException>(() => CallAsync(() => TestFuncAsync(counter, 10), ex => true));
            else
                Assert.Throws<AggregateException>(() => Call(() => TestFunc(counter, 10), ex => true));
            stopwatch.Stop();
            Assert.AreEqual(5, counter.Value, "counter");
            Assert.Greater(stopwatch.ElapsedMilliseconds, 500*(1 + 1.7 + 1.7*1.7 + 1.7*1.7*1.7) - 100, "ElapsedMilliseconds");
            Assert.Less(stopwatch.ElapsedMilliseconds, 500*(1 + 2.5 + 2.5*2.5 + 2.5*2.5*2.5) + 1000, "ElapsedMilliseconds");
        }

        [Test, TestCase(true), TestCase(false)]
        public void CallWithNonRetriableException_ReturnsWithowtDelays(bool async)
        {
            var counter = new AtomicInt(0);
            if (async)
                Assert.Throws<InvalidDataException>(() => CallAsync(() => TestFuncAsync(counter), ExceptionFinder.HasException<IndexOutOfRangeException>));
            else
                Assert.Throws<InvalidDataException>(() => Call(() => TestFunc(counter), ExceptionFinder.HasException<IndexOutOfRangeException>));
            stopwatch.Stop();
            Assert.Less(stopwatch.ElapsedMilliseconds, 50, "ElapsedMilliseconds");
            log.Info("elapsed = " + stopwatch.Elapsed);
        }

        private T CallAsync<T>(Func<Task<T>> action, Func<Exception, bool> isExceptionRetriable)
        {
            return callStrategy.CallAsync(action, isExceptionRetriable, log).GetAwaiter().GetResult();
        }

        private T Call<T>(Func<T> action, Func<Exception, bool> isExceptionRetriable)
        {
            return callStrategy.Call(action, isExceptionRetriable, log);
        }

        private async Task<int> TestFuncAsync(AtomicInt counter, int maxCounterWithException = 2)
        {
            counter.Add(1);
            if (counter.Value <= maxCounterWithException)
                throw new InvalidDataException("something bad");
            await Task.Delay(1);
            return expectedResult;
        }

        private int TestFunc(AtomicInt counter, int maxCounterWithException = 2)
        {
            counter.Add(1);
            if (counter.Value <= maxCounterWithException)
                throw new InvalidDataException("something bad");
            return expectedResult;
        }
    }
}