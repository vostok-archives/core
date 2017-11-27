using System.Threading;

namespace Vostok.Hosting
{
    public class AsyncLocalVostokHostingContext : IVostokHostingContext
    {
        private static readonly AsyncLocal<IVostokHostingEnvironment> current = new AsyncLocal<IVostokHostingEnvironment>();
        public IVostokHostingEnvironment Current
        {
            get => current.Value;
            set => current.Value = value;
        }
    }
}