using System;

namespace Vstk.Flow.Serializers
{
    internal class GuidSerializer : BaseTypedSerializer<Guid>
    {
        public override string Id => "guid";

        protected override bool TrySerialize(Guid value, out string serializedValue)
        {
            serializedValue = value.ToString("D");
            return true;
        }

        protected override bool TryDeserialize(string serializedValue, out Guid value)
            => Guid.TryParse(serializedValue, out value);
    }
}
