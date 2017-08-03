using System;

namespace Vostok.Flow
{
    public static class IContextPropertiesExtensions_Scopes
    {
        public static IDisposable Use<TValue>(this IContextProperties properties, string key, TValue value)
        {
            object oldValue;
            properties.Current.TryGetValue(key, out oldValue);
            properties.SetProperty(key, value);
            return new ContextScope(key, oldValue, properties);
        }
    }
}
