namespace Vostok.Hosting
{
    public class StaticVostokHostingContext : IVostokHostingContext
    {
        private static IVostokHostingEnvironment current;
        public IVostokHostingEnvironment Current
        {
            get => current;
            set => current = value;
        }
    }
}