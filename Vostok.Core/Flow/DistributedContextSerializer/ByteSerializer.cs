namespace Vostok.Flow.DistributedContextSerializer
{
    public class ByteSerializer : BaseTypedSerializer<byte>
    {
        protected override bool TryDeserialize(string serializedValue, out byte value)
            => byte.TryParse(serializedValue, out value);
    }
}