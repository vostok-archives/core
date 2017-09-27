using System;

namespace Vostok.Metrics
{
    public class GaugeMeter : IMeter<double>
    {
        private readonly Func<double> getValue;

        public GaugeMeter(Func<double> getValue)
        {
            this.getValue = getValue;
        }

        public double Reset()
        {
            return getValue();
        }
    }
}