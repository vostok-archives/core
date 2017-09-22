using System.Globalization;

namespace Vostok.Flow.Serializers
{
    internal class ByteSerializer : BaseTypedSerializer<byte>
    {
        public override string Id => "ubyte";

        protected override bool TrySerialize(byte value, out string serializedValue)
        {
            serializedValue = value.ToString(CultureInfos.EnUs);
            return true;
        }

        protected override bool TryDeserialize(string serializedValue, out byte value)
            => byte.TryParse(serializedValue, NumberStyles.Any, CultureInfos.EnUs, out value);
    }
}