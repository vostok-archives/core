using System;

namespace Vostok.Clusterclient.Helpers
{
    internal static class ExceptionAssertions
    {
        public static void ShouldBePrinted<T>(this T exception)
            where T : Exception
        {
            Console.Out.WriteLine(exception);
        }
    }
}
