using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using Vostok.Airlock.Logging;
using Vostok.Logging;
using Vostok.Logging.Logs;

namespace Vostok.Airlock.Client.Tests.Logging
{
    public class ExceptionParser_Tests
    {
        private static readonly object[] testCases =
        {
            new object[] {"DivideByZero", (Action)DivideByZero, new[] {"TestByThrowingException", "DivideByZero"}, new[] {"System.DivideByZeroException"}},
            new object[] {"MyGenericFunc<int>", (Action)MyGenericFunc<int>, new[] {"TestByThrowingException", "MyGenericFunc"}, new[] {"System.Exception"}},
            new object[] {"GenericClassFunc", (Action)GenericClassFunc, new[] {"TestByThrowingException", "GenericClassFunc", "MyFunc"}, new[] {"System.Exception"}},
            new object[] {"AsyncFunc", (Action)(() => MyAsyncFunc().GetAwaiter().GetResult()), new[] {"TestByThrowingException", "<.cctor>b__12_0", "HandleNonSuccessAndDebuggerNotification", "Throw", "MyAsyncFunc"}, new[] {"System.Exception"}},
            new object[] {"LambdaFunc", (Action)MyLambdaFunc, new[] {"TestByThrowingException", "MyLambdaFunc", "MyLambdaFunc { <lambda> }"}, new[] {"System.Exception"}},
            new object[] {"NestedFunc", (Action)NestedFunc, new[] {"NestedFunc", "NestedFunc2", "TestByThrowingException", "NestedFunc"}, new[] {"System.InvalidOperationException", "System.IO.InvalidDataException"}},
            new object[] {"AggregateException", (Action)AggregateExceptionFunc, new[] {"AggregateExceptionFunc", "AggregateExceptionFunc", "TestByThrowingException", "AggregateExceptionFunc"}, new[] {"System.AggregateException", "System.IO.InvalidDataException", "System.InvalidOperationException"}}
        };

        private static readonly string[][] asyncNameVariants =
        {
            new[] {"TestByThrowingException", "<.cctor>b__12_0", "HandleNonSuccessAndDebuggerNotification", "Throw", "MyAsyncFunc"},
            new[] {"TestByThrowingException", "<.cctor>b__12_0", "GetResult", "HandleNonSuccessAndDebuggerNotification", "Throw", "MyAsyncFunc"},
            new[] {"TestByThrowingException", "<.cctor>b__12_0", "HandleNonSuccessAndDebuggerNotification", "ThrowForNonSuccess", "MyAsyncFunc"}
        };

        private readonly ConsoleLog log = new ConsoleLog();

        [TestCaseSource(nameof(testCases))]
        public void TestByThrowingException(string caseName, Action action, string[] funcNames, string[] exNames)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                log.Error(e);
                var logEventExceptions = e.Parse();
                log.Debug("got:\n" + JsonConvert.SerializeObject(logEventExceptions, Formatting.Indented));
                var funcNamesAtException = logEventExceptions.SelectMany(e1 => e1.Stack).Select(x => x.Function).Reverse();
                if (caseName != "AsyncFunc")
                    funcNamesAtException.ShouldAllBeEquivalentTo(funcNames, c => c.WithStrictOrderingFor(x => x), "funcNames for case " + caseName);
                else
                    Assert.That(asyncNameVariants.Any(asyncVariant => asyncVariant.SequenceEqual(funcNames)));
                logEventExceptions.Select(ex => ex.Type).ShouldAllBeEquivalentTo(exNames, c => c.WithStrictOrderingFor(x => x), "exception name for case " + caseName);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void MyGenericFunc<T>()
        {
            throw new Exception("hello from generic func " + typeof (T).Name);
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
        [SuppressMessage("ReSharper", "UnusedVariable")]
        private static void DivideByZero()
        {
            var i2 = 0;
            var i = 10/i2;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void GenericClassFunc()
        {
            new MyClass<double>().MyFunc();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void AggregateExceptionFunc()
        {
            var exceptions = new List<Exception>();
            try
            {
                throw new InvalidDataException("no data");
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
            try
            {
                throw new InvalidOperationException("no oper");
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
            throw new AggregateException(exceptions);
        }

        private class MyClass<T>
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            public void MyFunc()
            {
                throw new Exception("hello from generic class " + typeof (T).Name);
            }
        }
    }
}