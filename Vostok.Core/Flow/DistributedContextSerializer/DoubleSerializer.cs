namespace Vostok.Flow.DistributedContextSerializer
{
    public class DoubleSerializer : BaseTypedSerializer<double>
    {
        protected override bool TryDeserialize(string serializedValue, out double value)
            => double.TryParse(serializedValue, out value);
    }
}