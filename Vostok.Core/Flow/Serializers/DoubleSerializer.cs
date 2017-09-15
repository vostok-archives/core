namespace Vostok.Flow.Serializers
{
    // CR(iloktionov): Для типов, у которых есть вариации в формате (зависящие от локали, культуры), надо жестко зафиксировать формат (который, в идеале, и джава поймет).
    public class DoubleSerializer : BaseTypedSerializer<double>
    {
        public override string Id => "double";

        protected override bool TryDeserialize(string serializedValue, out double value)
            => double.TryParse(serializedValue, out value);
    }
}