namespace Vostok.Flow.DistributedContextSerializer
{
    public class StringSerializer : BaseTypedSerializer<string>
    {
        protected override bool TryDeserialize(string serializedValue, out string value)
        {
            value = serializedValue;
            return true;
        }
    }
}