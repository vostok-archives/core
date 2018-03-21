namespace Vostok.Commons.Threading
{
    public class ThreadPoolState
    {
        public int MinWorkerThreads { get; set; }
        public int MinIocpThreads { get; set; }

        public int MaxWorkerThreads { get; set; }
        public int MaxIocpThreads { get; set; }

        public int UsedWorkerThreads { get; set; }
        public int UsedIocpThreads { get; set; }
    }
}