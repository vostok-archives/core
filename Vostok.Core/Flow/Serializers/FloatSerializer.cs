using System.Globalization;

namespace Vostok.Flow.Serializers
{
    internal class FloatSerializer : BaseTypedSerializer<float>
    {
        public override string Id => "float";

        protected override bool TrySerialize(float value, out string serializedValue)
        {
            serializedValue = value.ToString(CultureInfoExtensions.EnUs);
            return true;
        }

        protected override bool TryDeserialize(string serializedValue, out float value)
            => float.TryParse(serializedValue, NumberStyles.Any, CultureInfoExtensions.EnUs, out value);
    }
}
