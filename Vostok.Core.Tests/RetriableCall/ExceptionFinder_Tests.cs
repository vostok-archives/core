using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Vstk.RetriableCall
{
    public class ExceptionFinder_Tests
    {
        private static IEnumerable<object[]> GenerateTestCases()
        {
            var invalidDataException = new InvalidDataException("invdata");
            var invalidOperationException = new InvalidOperationException("invop");
            var aggregateException = new AggregateException("agg", invalidOperationException, invalidDataException);
            yield return new object[] { "rootEx", aggregateException, (Func<Exception, bool>) (ex => ex is AggregateException), aggregateException };
            yield return new object[] { "abscentEx", aggregateException, (Func<Exception, bool>) (ex => ex is OutOfMemoryException), null };
            yield return new object[] { "condition", aggregateException, (Func<Exception, bool>) (ex => ex.Message == "invdata"), invalidDataException };
            yield return new object[] { "wrongCondition", aggregateException, (Func<Exception, bool>) (ex => ex.Message == "sdfkjdhslkj"), null };
            var invalidOperationException2 = new InvalidOperationException("op2");
            var aggregateException2 = new AggregateException("agg2", aggregateException, invalidOperationException2);
            var complexEx = new ArgumentException("arg", aggregateException2);
            yield return new object[] { "nestedEx", complexEx, (Func<Exception, bool>) (ex => ex is InvalidOperationException), invalidOperationException };
            yield return new object[] { "nestedEx", complexEx, (Func<Exception, bool>) (ex => ex is AggregateException), aggregateException2 };
        }

        [TestCaseSource(nameof(GenerateTestCases))]
        public void FindFirstTests(string testCase, Exception ex, Func<Exception,bool> condition, Exception result)
        {
            var firstException = ex.FindFirstException(condition);
            Assert.AreEqual(result, firstException);
        }
    }
}