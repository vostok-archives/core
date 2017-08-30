using System;

namespace Vostok.Clusterclient.Ordering.Weighed.Adaptive
{
    public struct HealthWithDecay
    {
        public readonly double Value;
        public readonly DateTime DecayPivot;

        public HealthWithDecay(double value, DateTime decayPivot)
        {
            Value = value;
            DecayPivot = decayPivot;
        }
    }
}
