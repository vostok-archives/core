using System;

namespace Vostok.Flow.DistributedContextSerializer
{
    internal class GuidSerializer : BaseTypedSerializer<Guid>
    {
        public override string Id => "guid";

        protected override bool TryDeserialize(string serializedValue, out Guid value)
            => Guid.TryParse(serializedValue, out value);
    }
}