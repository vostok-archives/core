namespace Vostok.Flow
{
    public static class Context
    {
        static Context()
        {
            Properties = new ContextProperties();
            Configuration = new DistributedContextConfiguration();
        }

        public static IContextProperties Properties { get; }

        public static IDistributedContextConfiguration Configuration { get; }
    }
}
