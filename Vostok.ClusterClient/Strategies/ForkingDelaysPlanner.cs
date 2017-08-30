using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vostok.Clusterclient.Strategies
{
    internal class ForkingDelaysPlanner : IForkingDelaysPlanner
    {
        public static readonly ForkingDelaysPlanner Instance = new ForkingDelaysPlanner();

        public Task Plan(TimeSpan delay, CancellationToken token)
        {
            return Task.Delay(delay, token);
        }
    }
}
