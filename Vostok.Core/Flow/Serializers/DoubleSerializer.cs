using System.Globalization;

namespace Vostok.Flow.Serializers
{
    internal class DoubleSerializer : BaseTypedSerializer<double>
    {
        public override string Id => "double";

        protected override bool TrySerialize(double value, out string serializedValue)
        {
            serializedValue = value.ToString(CultureInfos.EnUs);
            return true;
        }

        protected override bool TryDeserialize(string serializedValue, out double value)
            => double.TryParse(serializedValue, NumberStyles.Any, CultureInfos.EnUs, out value);
    }
}