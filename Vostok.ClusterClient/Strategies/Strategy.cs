using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Sending;
using Vostok.Clusterclient.Strategies.DelayProviders;
using Vostok.Clusterclient.Strategies.TimeoutProviders;

namespace Vostok.Clusterclient.Strategies
{
    public sealed class Strategy : IRequestStrategy
    {
        /// <summary>
        /// Returns an instance of <see cref="SingleReplicaRequestStrategy"/>.
        /// </summary>
        public static readonly SingleReplicaRequestStrategy SingleReplica = new SingleReplicaRequestStrategy();

        /// <summary>
        /// Returns an instance of <see cref="SequentialRequestStrategy"/> with an <see cref="EqualTimeoutsProvider"/> whose division factor is set to 1.
        /// </summary>
        public static readonly SequentialRequestStrategy Sequential1 = Sequential(1);

        /// <summary>
        /// Returns an instance of <see cref="SequentialRequestStrategy"/> with an <see cref="EqualTimeoutsProvider"/> whose division factor is set to 2.
        /// </summary>
        public static readonly SequentialRequestStrategy Sequential2 = Sequential(2);

        /// <summary>
        /// Returns an instance of <see cref="SequentialRequestStrategy"/> with an <see cref="EqualTimeoutsProvider"/> whose division factor is set to 3.
        /// </summary>
        public static readonly SequentialRequestStrategy Sequential3 = Sequential(3);

        /// <summary>
        /// Returns an instance of <see cref="ParallelRequestStrategy"/> with parallelism level = 2.
        /// </summary>
        public static readonly ParallelRequestStrategy Parallel2 = Parallel(2);

        /// <summary>
        /// Returns an instance of <see cref="ParallelRequestStrategy"/> with parallelism level = 3.
        /// </summary>
        public static readonly ParallelRequestStrategy Parallel3 = Parallel(3);

        /// <summary>
        /// Returns an instance of <see cref="ParallelRequestStrategy"/> with unlimited parallelism level.
        /// </summary>
        public static readonly ParallelRequestStrategy ParallelUnlimited = Parallel(int.MaxValue);

        /// <summary>
        /// Returns an instance of <see cref="ParallelRequestStrategy"/> with max parallelism = 2 and an <see cref="EqualDelaysProvider"/> with division factor = 2.
        /// </summary>
        public static readonly ForkingRequestStrategy Forking2 = Forking(2);

        /// <summary>
        /// Returns an instance of <see cref="ParallelRequestStrategy"/> with max parallelism = 3 and an <see cref="EqualDelaysProvider"/> with division factor = 3.
        /// </summary>
        public static readonly ForkingRequestStrategy Forking3 = Forking(3);

        /// <summary>
        /// Creates an instance of <see cref="SequentialRequestStrategy"/> with given <paramref name="timeoutsProvider"/>.
        /// </summary>
        public static SequentialRequestStrategy Sequential(ISequentialTimeoutsProvider timeoutsProvider)
        {
            return new SequentialRequestStrategy(timeoutsProvider);
        }

        /// <summary>
        /// Creates an instance of <see cref="SequentialRequestStrategy"/> with <see cref="EqualTimeoutsProvider"/> whose timeout division factor will be set to <paramref name="divisionFactor"/>.
        /// </summary>
        public static SequentialRequestStrategy Sequential(int divisionFactor)
        {
            return Sequential(new EqualTimeoutsProvider(divisionFactor));
        }

        /// <summary>
        /// Creates an instance of <see cref="ParallelRequestStrategy"/> with given <paramref name="parallelismLevel"/>.
        /// </summary>
        public static ParallelRequestStrategy Parallel(int parallelismLevel)
        {
            return new ParallelRequestStrategy(parallelismLevel);
        }

        /// <summary>
        /// Creates an instance of <see cref="ForkingRequestStrategy"/> with given <paramref name="maximumParallelism"/> and <paramref name="delaysProvider"/>.
        /// </summary>
        public static ForkingRequestStrategy Forking(IForkingDelaysProvider delaysProvider, int maximumParallelism)
        {
            return new ForkingRequestStrategy(delaysProvider, maximumParallelism);
        }

        /// <summary>
        /// Creates an instance of <see cref="ForkingRequestStrategy"/> with given <paramref name="maximumParallelism"/> and <see cref="EqualDelaysProvider"/> with division factor = <paramref name="maximumParallelism"/>.
        /// </summary>
        public static ForkingRequestStrategy Forking(int maximumParallelism)
        {
            return new ForkingRequestStrategy(new EqualDelaysProvider(maximumParallelism), maximumParallelism);
        }

        private Strategy()
        {
        }

        #region IRequestStrategy implementation stub

        public Task SendAsync(Request request, IRequestSender sender, IRequestTimeBudget budget, IEnumerable<Uri> replicas, int replicasCount, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
