using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using Vostok.Airlock.Logging;
using Vostok.Logging.Logs;

namespace Vostok.Logging
{
    public class ExceptionParsingTests
    {
        private static readonly object[] testCases =
        {
            new object[] {"DivideByZero", (Action)DivideByZero, new[] {"TestByThrowingException", "DivideByZero"}, new[] { "System.DivideByZeroException" } },
            new object[] {"MyGenericFunc<int>", (Action)MyGenericFunc<int>, new[] {"TestByThrowingException", "MyGenericFunc"}, new[] { "System.Exception" } },
            new object[] {"GenericClassFunc", (Action)GenericClassFunc, new[] {"TestByThrowingException", "GenericClassFunc", "MyFunc"}, new[] { "System.Exception" } },
            new object[] {"AsyncFunc", (Action)(() => MyAsyncFunc().GetAwaiter().GetResult()), new[] {"TestByThrowingException", "<.cctor>b__12_0", "HandleNonSuccessAndDebuggerNotification", "Throw", "MyAsyncFunc"}, new[] { "System.Exception" } },
            new object[] {"LambdaFunc", (Action)MyLambdaFunc, new[] {"TestByThrowingException", "MyLambdaFunc", "MyLambdaFunc { <lambda> }"}, new[] { "System.Exception" } },
            new object[] {"NestedFunc", (Action)NestedFunc, new[] {"TestByThrowingException", "NestedFunc", "NestedFunc", "NestedFunc2"}, new[] { "System.InvalidOperationException", "System.IO.InvalidDataException" } }
        };

        private static readonly string[][] asyncNameVariants =
        {
            new[] {"TestByThrowingException", "<.cctor>b__12_0", "HandleNonSuccessAndDebuggerNotification", "Throw", "MyAsyncFunc"},
            new[] {"TestByThrowingException", "<.cctor>b__12_0", "GetResult", "HandleNonSuccessAndDebuggerNotification", "Throw", "MyAsyncFunc"},
            new[] {"TestByThrowingException", "<.cctor>b__12_0", "HandleNonSuccessAndDebuggerNotification", "ThrowForNonSuccess", "MyAsyncFunc"}
        };

        private readonly ConsoleLog log = new ConsoleLog();

        //[TestCaseSource(nameof(testCases))]
        [Test]
        public void TestByThrowingException() //Action action, string[] funcNames, string[] exNames
        {
            foreach (var testCase in testCases.Cast<object[]>())
            {
                var caseName = (string)testCase[0];
                var action = (Action)testCase[1];
                var funcNames = (string[])testCase[2];
                var exNames = (string[])testCase[3];
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    log.Error(e);
                    var logEventData = new LogEventData(e);
                    log.Debug("got:\n" + JsonConvert.SerializeObject(logEventData, Formatting.Indented));
                    var funcNamesAtException = logEventData.Exceptions.SelectMany(e1 => e1.Stack).Select(x => x.Function).Reverse();
                    if (caseName != "AsyncFunc")
                    {
                        funcNamesAtException.ShouldAllBeEquivalentTo(funcNames, "funcNames for case " + caseName);
                    }
                    else
                    {
                        Assert.That(asyncNameVariants.Any(asyncVariant => asyncVariant.SequenceEqual(funcNames)));
                    }
                    logEventData.Exceptions.Select(ex => ex.Type).ShouldAllBeEquivalentTo(exNames, "exception name for case " + caseName);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void MyGenericFunc<T>()
        {
            throw new Exception("hello from generic func " + typeof(T).Name);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static async Task MyAsyncFunc()
        {
            await Task.Delay(200);
            throw new Exception("hello from async func");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void MyLambdaFunc()
        {
            var action = new Action(
                () => throw new Exception("hello from lambda"));
            action();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void NestedFunc()
        {
            try
            {
                NestedFunc2();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("invalid oper", e);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void NestedFunc2()
        {
            throw new InvalidDataException("bad data");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void DivideByZero()
        {
            var i2 = 0;
            // ReSharper disable once UnusedVariable
            var i = 10 / i2;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void GenericClassFunc()
        {
            new MyClass<double>().MyFunc();
        }

        private class MyClass<T>
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            public void MyFunc()
            {
                throw new Exception("hello from generic class " + typeof(T).Name);
            }
        }
    }
}