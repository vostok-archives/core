using System.Collections.Generic;
using System.Threading;

namespace Vostok.Flow
{
    internal class ContextProperties : IContextProperties
    {
        private readonly AsyncLocal<ContextPropertiesSnapshot> container;

        public ContextProperties()
        {
            container = new AsyncLocal<ContextPropertiesSnapshot>();
        }

        public IReadOnlyDictionary<string, object> Current => Properties;

        private ContextPropertiesSnapshot Properties
        {
            get => container.Value ?? ContextPropertiesSnapshot.Empty;
            set => container.Value = value;
        }

        public void SetProperty(string key, object value)
        {
            Properties = Properties.Set(key, value);
        }

        public void RemoveProperty(string key)
        {
            Properties = Properties.Remove(key);
        }
    }
}
