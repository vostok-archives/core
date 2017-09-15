namespace Vostok.Flow.Serializers
{
    // CR(iloktionov): 1. Идентификаторы типов надо бы сделать такими, чтобы не пострадал interop с джавой (например, там byte - число со знаком, то есть вот это - "ubyte").
    // CR(iloktionov): 2. Забыли bool.
    public class ByteSerializer : BaseTypedSerializer<byte>
    {
        public override string Id => "byte";

        protected override bool TryDeserialize(string serializedValue, out byte value)
            => byte.TryParse(serializedValue, out value);
    }
}