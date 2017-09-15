namespace Vostok.Flow.DistributedContextSerializer
{
    public class LongSerializer : BaseTypedSerializer<long>
    {
        public override string Id => "long";

        protected override bool TryDeserialize(string serializedValue, out long value)
            => long.TryParse(serializedValue, out value);
    }
}