namespace Vostok.Flow.Serializers
{
    internal class StringSerializer : BaseTypedSerializer<string>
    {
        public override string Id => "string";

        protected override bool TrySerialize(string value, out string serializedValue)
        {
            serializedValue = value;
            return true;
        }

        protected override bool TryDeserialize(string serializedValue, out string value)
        {
            value = serializedValue;
            return true;
        }
    }
}