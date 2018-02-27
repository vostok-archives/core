namespace Vstk.Flow.Serializers
{
    internal class CharSerializer : BaseTypedSerializer<char>
    {
        public override string Id => "char";

        protected override bool TrySerialize(char value, out string serializedValue)
        {
            serializedValue = value.ToString();
            return true;
        }

        protected override bool TryDeserialize(string serializedValue, out char value)
            => char.TryParse(serializedValue, out value);
    }
}