using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Vostok.Airlock.Logging
{
    // todo (andrew, 17.01.2018): need to test this thoroughly (how all fields are filled with respect to "pdbonly" and "full" debugging information types)
    public static class ExceptionParser
    {
        // todo (andrew, 17.01.2018): maybe get rid of regex-es for optimization
        private static readonly Regex asyncRegex = new Regex(@"^(.*)[\.\+]<(\w*)>d__\d*", RegexOptions.Compiled);
        private static readonly Regex lambdaRegex = new Regex(@"^<?(\w*)>b__\w+", RegexOptions.Compiled);

        public static List<LogEventException> Parse(this Exception ex)
        {
            return GetExceptions(ex).Select(CreateLogEventException).ToList();
        }

        private static IEnumerable<Exception> GetExceptions(Exception ex)
        {
            var currentEx = ex;
            while (currentEx != null)
            {
                yield return currentEx;
                var innerExceptions = TryGetInnerExceptions(currentEx);
                if (innerExceptions.Exceptions != null)
                {
                    foreach (var innerException in innerExceptions.Exceptions)
                    {
                        foreach (var e in GetExceptions(innerException))
                            yield return e;
                    }
                }
                if (innerExceptions.TerminateInnerExceptionTraversal)
                    yield break;
                currentEx = currentEx.InnerException;
            }
        }

        private static (IEnumerable<Exception> Exceptions, bool TerminateInnerExceptionTraversal) TryGetInnerExceptions(Exception ex)
        {
            switch (ex)
            {
                case AggregateException aggregateException:
                    return (aggregateException.InnerExceptions, true); // AggregateException.InnerException == AggregateException.InnerExceptions[0]
                case ReflectionTypeLoadException typeLoadException:
                    return (typeLoadException.LoaderExceptions, false);
                default:
                    return (null, false);
            }
        }

        private static LogEventException CreateLogEventException(Exception ex)
        {
            var logEventException = new LogEventException
            {
                Module = ex.Source,
                Message = ex.Message,
                Type = ex.GetType().FullName
            };
            var frames = new StackTrace(ex, true).GetFrames();
            if (frames != null)
                logEventException.Stack = frames.Select(CreateLogEventStackFrame).ToList();
            return logEventException;
        }

        private static LogEventStackFrame CreateLogEventStackFrame(StackFrame frame)
        {
            var stackFrame = new LogEventStackFrame();
            var method = frame.GetMethod();
            if (method != null)
            {
                stackFrame.Module = method.DeclaringType != null ? method.DeclaringType.FullName : null;
                stackFrame.Function = method.Name;
                stackFrame.Source = method.ToString();
            }
            else
            {
                stackFrame.Module = "(unknown)";
                stackFrame.Function = "(unknown)";
                stackFrame.Source = "(unknown)";
            }
            stackFrame.Filename = frame.GetFileName();
            stackFrame.LineNumber = frame.GetFileLineNumber();
            stackFrame.ColumnNumber = frame.GetFileColumnNumber();
            FixNames(stackFrame);
            return stackFrame;
        }

        private static void FixNames(LogEventStackFrame stackFrame)
        {
            if (stackFrame.Function == "MoveNext")
            {
                var asyncMatch = asyncRegex.Match(stackFrame.Module);
                if (asyncMatch.Success)
                {
                    stackFrame.Module = asyncMatch.Groups[1].Value;
                    stackFrame.Function = asyncMatch.Groups[2].Value;
                }
            }
            var matchLambda = lambdaRegex.Match(stackFrame.Function);
            if (matchLambda.Success)
                stackFrame.Function = matchLambda.Groups[1].Value + " { <lambda> }";
            if (stackFrame.Module.EndsWith(".<>c") || stackFrame.Module.EndsWith("+<>c"))
                stackFrame.Module = stackFrame.Module.Substring(0, stackFrame.Module.Length - 4);
        }
    }
}