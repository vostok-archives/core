using System;

namespace Vostok.Flow.DistributedContextSerializer
{
    public class ByteArraySerializer : BaseTypedSerializer<byte[]>
    {
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