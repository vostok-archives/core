using System;

namespace Vostok.Metrics
{
    public static class IMeterScopeExtensions
    {
        public static SumMeter CreateSumMeter(this IMeterScope scope, TimeSpan period)
        {
            var sumMeter = new SumMeter();
            scope.Register(sumMeter, null, period);
            return sumMeter;
        }
    }
}