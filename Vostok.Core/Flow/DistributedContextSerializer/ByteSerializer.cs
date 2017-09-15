namespace Vostok.Flow.DistributedContextSerializer
{
    public class ByteSerializer : BaseTypedSerializer<byte>
    {
        public override string Id => "byte";

        protected override bool TryDeserialize(string serializedValue, out byte value)
            => byte.TryParse(serializedValue, out value);
    }
}