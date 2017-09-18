using System.Globalization;

namespace Vostok.Flow.Serializers
{
    internal class LongSerializer : BaseTypedSerializer<long>
    {
        public override string Id => "int64";

        protected override bool TrySerialize(long value, out string serializedValue)
        {
            serializedValue = value.ToString(CultureInfoExtensions.EnUs);
            return true;
        }

        protected override bool TryDeserialize(string serializedValue, out long value)
            => long.TryParse(serializedValue, NumberStyles.Any, CultureInfoExtensions.EnUs, out value);
    }
}
