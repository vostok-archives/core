using System.Globalization;

namespace Vstk.Flow.Serializers
{
    internal class FloatSerializer : BaseTypedSerializer<float>
    {
        public override string Id => "float";

        protected override bool TrySerialize(float value, out string serializedValue)
        {
            serializedValue = value.ToString(CultureInfos.EnUs);
            return true;
        }

        protected override bool TryDeserialize(string serializedValue, out float value)
            => float.TryParse(serializedValue, NumberStyles.Any, CultureInfos.EnUs, out value);
    }
}
