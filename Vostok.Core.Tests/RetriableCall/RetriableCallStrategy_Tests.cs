using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Vostok.Commons.Synchronization;
using Vostok.Logging;
using Vostok.Logging.Logs;

namespace Vostok.RetriableCall
{
    [TestFixture]
    public class RetriableCallStrategy_Tests
    {
        private const int expectedResult = 10;
        private readonly ConsoleLog log = new ConsoleLog();
        private readonly RetriableCallStrategy callStrategy = new RetriableCallStrategy();
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

        [Test]
        public void CallAsyncWithoutErrors_ReturnsWithowtDelays()
        {
            callStrategy.CallAsync(() => Task.CompletedTask, ex => true, log).GetAwaiter().GetResult();
            Assert.Less(stopwatch.ElapsedMilliseconds, 50, "ElapsedMilliseconds");
            stopwatch.Restart();
            var actualResult = CallAsync(() => Task.FromResult(expectedResult), ex => true);
            Assert.Less(stopwatch.ElapsedMilliseconds, 50, "ElapsedMilliseconds");
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void CallWithoutErrors_ReturnsWithowtDelays()
        {
            callStrategy.Call(() => 0, ex => true, log);
            Assert.Less(stopwatch.ElapsedMilliseconds, 50, "ElapsedMilliseconds");
            log.Info("elapsed = " + stopwatch.Elapsed);
            stopwatch.Restart();
            var actualResult = Call(() => expectedResult, ex => true);
            Assert.Less(stopwatch.ElapsedMilliseconds, 50, "ElapsedMilliseconds");
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void CallAsyncWithErrors_ReturnsErrorOrSuccess()
        {
            var counter = new AtomicInt(0);
            var actualResult = CallAsync(() => TestFuncAsync(counter), ex => true);
            stopwatch.Stop();
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(3, counter.Value,"counter");
            Assert.Greater(stopwatch.ElapsedMilliseconds, 500*(1+1.7)-100, "ElapsedMilliseconds");
            Assert.Less(stopwatch.ElapsedMilliseconds, 500*(1+2.5)+100, "ElapsedMilliseconds");
        }

        [Test]
        public void CallWithErrors_ReturnsErrorOrSuccess()
        {
            var counter = new AtomicInt(0);
            var actualResult = Call(() => TestFunc(counter), ex => true);
            stopwatch.Stop();
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(3, counter.Value,"counter");
            Assert.Greater(stopwatch.ElapsedMilliseconds, 500*(1+1.7)-100, "ElapsedMilliseconds");
            Assert.Less(stopwatch.ElapsedMilliseconds, 500*(1+2.5)+100, "ElapsedMilliseconds");
        }

        [Test]
        public void CallAsyncWithNonRetriableException_ReturnsWithowtDelays()
        {
            var counter = new AtomicInt(0);
            Assert.Throws<InvalidDataException>(() => CallAsync(() => TestFuncAsync(counter), ExceptionFinder.HasException<IndexOutOfRangeException>));
            stopwatch.Stop();
            Assert.Less(stopwatch.ElapsedMilliseconds, 50, "ElapsedMilliseconds");
            log.Info("elapsed = " + stopwatch.Elapsed);
        }

        [Test]
        public void CallWithNonRetriableException_ReturnsWithowtDelays()
        {
            var counter = new AtomicInt(0);
            Assert.Throws<InvalidDataException>(() => Call(() => TestFunc(counter), ExceptionFinder.HasException<IndexOutOfRangeException>));
            stopwatch.Stop();
            Assert.Less(stopwatch.ElapsedMilliseconds, 50, "ElapsedMilliseconds");
            log.Info("elapsed = " + stopwatch.Elapsed);
        }

        private T CallAsync<T>(Func<Task<T>> action, Func<Exception, bool> isExceptionRetriable)
        {
            return callStrategy.CallAsync<T>(action, isExceptionRetriable, log).GetAwaiter().GetResult();
        }
        private T Call<T>(Func<T> action, Func<Exception, bool> isExceptionRetriable)
        {
            return callStrategy.Call(action, isExceptionRetriable, log);
        }

        private async Task<int> TestFuncAsync(AtomicInt counter)
        {
            counter.Add(1);
            if (counter.Value < 3)
                throw new InvalidDataException("something bad");
            await Task.Delay(1);
            return expectedResult;
        }
        private int TestFunc(AtomicInt counter)
        {
            counter.Add(1);
            if (counter.Value < 3)
                throw new InvalidDataException("something bad");
            return expectedResult;
        }
    }
}