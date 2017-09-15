using System;

namespace Vostok.Flow.Serializers
{
    internal class BytesArraySerializer : BaseTypedSerializer<byte[]>
    {
        public override string Id => "BytesArray";

        protected override bool TrySerialize(byte[] value, out string serializedValue)
        {
            serializedValue = Convert.ToBase64String(value);
            return true;
        }

        protected override bool TryDeserialize(string serializedValue, out byte[] value)
        {
            try
            {
                value = Convert.FromBase64String(serializedValue);
                return true;
            }
            catch (Exception)
            {
                value = null;
                return false;
            }
        }
    }
}