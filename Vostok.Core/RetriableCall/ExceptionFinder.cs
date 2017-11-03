using System;
using System.Linq;

namespace Vostok.RetriableCall
{
    public static class ExceptionFinder
    {
        public static bool HasException<TException>(Exception rootEx)
            where TException : Exception
        {
            return FindException<TException>(rootEx) != null;
        }

        public static TException FindException<TException>(Exception rootEx)
            where TException : Exception
        {
            return FindException(rootEx, e => e is TException) as TException;
        }

        public static TException FindException<TException>(Exception rootEx, Func<TException, bool> condition)
            where TException : Exception
        {
            return FindException(rootEx,
                ex => ex is TException te && condition(te)) as TException;
        }

        public static Exception FindException(Exception rootEx, Func<Exception, bool> condition)
        {
            var ex = rootEx;
            while (ex != null && !condition(ex))
                ex = GetChildEx(ex);
            return ex;
        }

        private static Exception GetChildEx(Exception ex)
        {
            return ex is AggregateException aggregateEx
                ? aggregateEx.InnerExceptions.LastOrDefault()
                : ex.InnerException;
        }
    }
}