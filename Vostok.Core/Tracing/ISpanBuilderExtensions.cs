namespace Vostok.Tracing
{
    public static class ISpanBuilderExtensions
    {
        public static void Cancel(this ISpanBuilder spanBuilder)
        {
            spanBuilder.IsCanceled = true;
        }

        public static void MakeEndless(this ISpanBuilder spanBuilder)
        {
            spanBuilder.IsEndless = true;
        }
    }
}
