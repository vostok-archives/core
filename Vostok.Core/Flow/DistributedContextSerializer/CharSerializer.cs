namespace Vostok.Flow.DistributedContextSerializer
{
    public class CharSerializer : BaseTypedSerializer<char>
    {
        public override string Id => "char";

        protected override bool TryDeserialize(string serializedValue, out char value)
            => char.TryParse(serializedValue, out value);
    }
}