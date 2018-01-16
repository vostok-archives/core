using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Vostok.Airlock.Logging
{
    public static class LogEventDataFactory
    {
        public static LogEventData CreateLogEventData(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException(nameof(ex));
            var logEventData = new LogEventData
            {
                Message = ex.Message,
                Exceptions = new List<LogEventException>()
            };
            logEventData.Exceptions.AddRange(SelectExceptions(ex).Select(CreateLogEventException));
            return logEventData;
        }

        private static IEnumerable<Exception> SelectExceptions(Exception ex)
        {
            var currentEx = ex;
            while (currentEx != null)
            {
                if (!(currentEx is AggregateException aggregateEx))
                {
                    yield return currentEx;
                    if (currentEx is ReflectionTypeLoadException typeLoadException)
                    {
                        foreach (var loaderException in typeLoadException.LoaderExceptions)
                            yield return loaderException;
                    }
                    currentEx = currentEx.InnerException;
                }
                else
                {
                    if (aggregateEx.InnerExceptions.Count == 1)
                    {
                        currentEx = aggregateEx.InnerException;
                        continue;
                    }

                    yield return aggregateEx;
                    foreach (var innerException in aggregateEx.InnerExceptions)
                    {
                        foreach (var logEventException in SelectExceptions(innerException))
                        {
                            yield return logEventException;
                        }
                    }
                    break;
                }

            }
        }

        private static LogEventException CreateLogEventException(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException(nameof(ex));
            var logEventException = new LogEventException
            {
                Module = ex.Source,
                Message = ex.Message,
                Type = ex.GetType().FullName
            };
            var frames = new StackTrace(ex, true).GetFrames();
            if (frames == null)
                return logEventException;
            logEventException.Stack = new List<LogEventStackFrame>();
            foreach (var frame in frames)
            {
                logEventException.Stack.Add(CreateLogEventStackFrame(frame));
            }
            return logEventException;
        }

        private static LogEventStackFrame CreateLogEventStackFrame(StackFrame frame)
        {
            var stackFrame = new LogEventStackFrame();
            var num = frame.GetFileLineNumber();
            if (num == 0)
                num = frame.GetILOffset();
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
            stackFrame.LineNumber = num;
            stackFrame.ColumnNumber = frame.GetFileColumnNumber();
            FixNames(stackFrame);
            return stackFrame;
        }

        private static readonly Regex asyncRegex = new Regex(@"^(.*)[\.\+]<(\w*)>d__\d*", RegexOptions.Compiled);
        private static readonly Regex lambdaRegex = new Regex(@"^<?(\w*)>b__\w+", RegexOptions.Compiled);

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
            {
                stackFrame.Function = matchLambda.Groups[1].Value + " { <lambda> }";
            }
            if (stackFrame.Module.EndsWith(".<>c") || stackFrame.Module.EndsWith("+<>c"))
                stackFrame.Module = stackFrame.Module.Substring(0, stackFrame.Module.Length - 4);
        }

    }
}