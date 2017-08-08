namespace Vostok.Airlock
{
    public static class AirlockSerializerRegistry
    {
        public static void Register<T>(IAirlockSerializer<T> serializer)
        {
            // ...
        }
    }
}