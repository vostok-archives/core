namespace Vstk.Tracing
{
    public static class SpanBuilder_Extensions
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
