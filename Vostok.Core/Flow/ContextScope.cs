using System;

namespace Vostok.Flow
{
    internal class ContextScope : IDisposable
    {
        private readonly string key;
        private readonly object oldValue;
        private readonly IContextProperties properties;

        public ContextScope(string key, object oldValue, IContextProperties properties)
        {
            this.key = key;
            this.oldValue = oldValue;
            this.properties = properties;
        }

        public void Dispose()
        {
            if (oldValue == null)
            {
                properties.RemoveProperty(key);
            }
            else
            {
                properties.SetProperty(key, oldValue);
            }
        }
    }
}
