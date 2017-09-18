using System.Globalization;

namespace Vostok.Flow.Serializers
{
    internal class ByteSerializer : BaseTypedSerializer<byte>
    {
        public override string Id => "ubyte";

        protected override bool TrySerialize(byte value, out string serializedValue)
        {
            serializedValue = value.ToString(CultureInfoExtensions.EnUs);
            return true;
        }

        protected override bool TryDeserialize(string serializedValue, out byte value)
            => byte.TryParse(serializedValue, NumberStyles.Any, CultureInfoExtensions.EnUs, out value);
    }

    // CR(iloktionov): Почему живет в одном файле с сериализатором? Причем тут extensions, если это не экстеншны?
    internal static class CultureInfoExtensions
    {
        public static CultureInfo EnUs = CultureInfo.GetCultureInfo("en-US");
    }
}