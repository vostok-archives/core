namespace Vostok.Flow.DistributedContextSerializer
{
    public class FloatSerializer : BaseTypedSerializer<float>
    {
        protected override bool TryDeserialize(string serializedValue, out float value)
            => float.TryParse(serializedValue, out value);
    }
}