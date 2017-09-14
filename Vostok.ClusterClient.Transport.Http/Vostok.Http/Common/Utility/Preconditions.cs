using System;
using System.Runtime.CompilerServices;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility
{
    internal static class Preconditions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureCondition(bool argumentCondition, string argumentName, string errorMessageFormat, params object[] errorMessageArgs)
        {
            if (!argumentCondition)
                throw new ArgumentException(String.Format(errorMessageFormat, errorMessageArgs), argumentName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureArgumentRange(bool argumentRangeCondition, string argumentName, string errorMessageFormat, params object[] errorMessageArgs)
        {
            if (!argumentRangeCondition)
                throw new ArgumentOutOfRangeException(argumentName, String.Format(errorMessageFormat, errorMessageArgs));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureNotNull<T>(T argument, string argumentName, string errorMessageFormat, params object[] errorMessageArgs) where T : class
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName, String.Format(errorMessageFormat, errorMessageArgs));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureNotNull<T>(T argument, string argumentName) where T : class
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);
        }
    }
}