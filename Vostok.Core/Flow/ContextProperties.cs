using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

namespace Vostok.Flow
{
    internal class ContextProperties : IContextProperties
    {
        private readonly AsyncLocal<ImmutableDictionary<string, object>> container;

        public ContextProperties()
        {
            container = new AsyncLocal<ImmutableDictionary<string, object>>();
        }

        public IReadOnlyDictionary<string, object> Current => Properties;

        public void SetProperty(string key, object value)
        {
            Properties = Properties.SetItem(key, value);
        }

        public void RemoveProperty(string key)
        {
            Properties = Properties.Remove(key);
        }

        private ImmutableDictionary<string, object> Properties
        {
            get => container.Value ?? ImmutableDictionary<string, object>.Empty;
            set => container.Value = value;
        }
    }
}