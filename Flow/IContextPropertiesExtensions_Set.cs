namespace Vostok.Flow
{
    public static class IContextPropertiesExtensions_Set
    {
        public static void Set<TValue>(this IContextProperties properties, string key, TValue value)
        {
            properties.SetProperty(key, value);
        }
    }
}