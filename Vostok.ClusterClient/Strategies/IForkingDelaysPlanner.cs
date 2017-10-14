using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vostok.Clusterclient.Strategies
{
    internal interface IForkingDelaysPlanner
    {
        Task Plan(TimeSpan delay, CancellationToken cancellationToken);
    }
}
