namespace Vostok.Flow
{
    public static class Context
    {
        static Context()
        {
            Properties = new ContextProperties();
            Configuration = new ContextConfiguration();
        }

        public static IContextProperties Properties { get; }

        public static IContextConfiguration Configuration { get; }
    }
}