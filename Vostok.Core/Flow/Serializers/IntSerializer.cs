using System.Globalization;

namespace Vostok.Flow.Serializers
{
    internal class IntSerializer : BaseTypedSerializer<int>
    {
        public override string Id => "int32";

        protected override bool TrySerialize(int value, out string serializedValue)
        {
            serializedValue = value.ToString(CultureInfos.EnUs);
            return true;
        }

        protected override bool TryDeserialize(string serializedValue, out int value)
            => int.TryParse(serializedValue, NumberStyles.Any, CultureInfos.EnUs, out value);
    }
}
