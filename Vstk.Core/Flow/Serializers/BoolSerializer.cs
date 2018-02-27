namespace Vstk.Flow.Serializers
{
    internal class BoolSerializer : BaseTypedSerializer<bool>
    {
        public override string Id => "bool";

        protected override bool TrySerialize(bool value, out string serializedValue)
        {
            serializedValue = value.ToString();
            return true;
        }

        protected override bool TryDeserialize(string serializedValue, out bool value)
            => bool.TryParse(serializedValue, out value);
    }
}
