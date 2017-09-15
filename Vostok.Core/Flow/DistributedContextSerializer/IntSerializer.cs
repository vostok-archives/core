namespace Vostok.Flow.DistributedContextSerializer
{
    public class IntSerializer : BaseTypedSerializer<int>
    {
        public override string Id => "int";

        protected override bool TryDeserialize(string serializedValue, out int value)
            => int.TryParse(serializedValue, out value);
    }
}