using System;

namespace Vstk.Flow.Serializers
{
    internal class BytesArraySerializer : BaseTypedSerializer<byte[]>
    {
        public override string Id => "byteArray";

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
            catch
            {
                value = null;
                return false;
            }
        }
    }
}
