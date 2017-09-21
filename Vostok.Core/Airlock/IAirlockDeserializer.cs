namespace Vostok.Airlock
{
    public interface IAirlockDeserializer<out T>
    {
        T Deserialize(IAirlockDeserializationSink sink);
    }
}