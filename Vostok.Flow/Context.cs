namespace Vostok.Flow
{
    public static class Context
    {
        static Context()
        {
            Properties = new ContextProperties();
        }

        public static IContextProperties Properties { get; }
    }
}