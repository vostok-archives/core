namespace Vostok.Flow
{
    public static class IContextPropertiesExtensions_Get
    {
        public static TValue Get<TValue>(this IContextProperties properties, string key, TValue defaultValue = default(TValue))
        {
            object value;
            return properties.Current.TryGetValue(key, out value) ? (TValue) value : defaultValue;
        }
    }
}