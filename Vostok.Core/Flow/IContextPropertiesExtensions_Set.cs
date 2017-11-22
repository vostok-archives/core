namespace Vostok.Flow
{
    public static class ContextPropertiesExtensions_Set
    {
        public static void Set<TValue>(this IContextProperties properties, string key, TValue value)
        {
            properties.SetProperty(key, value);
        }
    }
}
