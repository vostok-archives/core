namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.ToCore.ThreadManagement
{
    public struct ThreadPoolState
    {
        public ThreadPoolState(int minWorkerThreads, int usedThreads, int minIocpThreads, int usedIocpThreads)
            : this()
        {
            MinWorkerThreads = minWorkerThreads;
            UsedThreads = usedThreads;
            MinIocpThreads = minIocpThreads;
            UsedIocpThreads = usedIocpThreads;
        }

        public int MinWorkerThreads { get; set; }
        public int UsedThreads { get; set; }
        public int MinIocpThreads { get; set; }
        public int UsedIocpThreads { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is ThreadPoolState && Equals((ThreadPoolState)obj);
        }

        public bool Equals(ThreadPoolState other)
        {
            return MinWorkerThreads == other.MinWorkerThreads &&
                   UsedThreads == other.UsedThreads &&
                   MinIocpThreads == other.MinIocpThreads &&
                   UsedIocpThreads == other.UsedIocpThreads;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MinWorkerThreads;
                hashCode = (hashCode * 397) ^ UsedThreads;
                hashCode = (hashCode * 397) ^ MinIocpThreads;
                hashCode = (hashCode * 397) ^ UsedIocpThreads;
                return hashCode;
            }
        }
    }
}