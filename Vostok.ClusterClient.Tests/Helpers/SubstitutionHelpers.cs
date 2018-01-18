using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.Core;

namespace Vostok.ClusterClient.Tests.Helpers
{
    internal static class SubstitutionHelpers
    {
        public static ConfiguredCall ReturnsTask<T>(this Task<T> value, T returnThis, params T[] returnThese)
        {
            var fromResult = Task.FromResult(returnThis);
            var array = returnThese.Select(Task.FromResult).ToArray();
            return value.Returns(fromResult, array);
        }

        public static ConfiguredCall ReturnsTask<T>(this Task<T> value, Func<CallInfo, T> returnThis, params Func<CallInfo, T>[] returnThese)
        {
            var fromResult = GetTaskResultAdapter(returnThis);
            var array = returnThese.Select(GetTaskResultAdapter).ToArray();
            return value.Returns(fromResult, array);
        }

        public static ConfiguredCall ReturnsError<T>(this Task<T> value, Exception error)
        {
            var completionSource = new TaskCompletionSource<T>();
            completionSource.SetException(error);

            return value.Returns(completionSource.Task);
        }

        public static ConfiguredCall ReturnsError(this Task value, Exception error)
        {
            var completionSource = new TaskCompletionSource<bool>();
            completionSource.SetException(error);

            return value.Returns(completionSource.Task);
        }

        public static ConfiguredCall ReturnsTaskForAnyArgs<T>(this Task<T> value, T returnThis, params T[] returnThese)
        {
            var fromResult = Task.FromResult(returnThis);
            var array = returnThese.Select(Task.FromResult).ToArray();
            return value.ReturnsForAnyArgs(fromResult, array);
        }

        public static ConfiguredCall ReturnsTaskForAnyArgs<T>(this Task<T> value, Func<CallInfo, T> returnThis, params Func<CallInfo, T>[] returnThese)
        {
            var fromResult = GetTaskResultAdapter(returnThis);
            var array = returnThese.Select(GetTaskResultAdapter).ToArray();
            return value.ReturnsForAnyArgs(fromResult, array);
        }

        private static Func<CallInfo, Task<T>> GetTaskResultAdapter<T>(Func<CallInfo, T> returnThis)
        {
            return x => Task.FromResult(returnThis(x));
        }
    }
}
