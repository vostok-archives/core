using System;

namespace Vostok.Flow
{
    public static class IContextPropertiesExtensions_Scopes
    {
        public static IDisposable Use<TValue>(this IContextProperties properties, string key, TValue value)
        {
            properties.Current.TryGetValue(key, out var oldValue);
            properties.SetProperty(key, value);
            return new ContextScope(key, oldValue, properties);
        }
    }
}
