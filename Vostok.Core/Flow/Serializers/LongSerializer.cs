using System.Globalization;

namespace Vstk.Flow.Serializers
{
    internal class LongSerializer : BaseTypedSerializer<long>
    {
        public override string Id => "int64";

        protected override bool TrySerialize(long value, out string serializedValue)
        {
            serializedValue = value.ToString(CultureInfos.EnUs);
            return true;
        }

        protected override bool TryDeserialize(string serializedValue, out long value)
            => long.TryParse(serializedValue, NumberStyles.Any, CultureInfos.EnUs, out value);
    }
}
