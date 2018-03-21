using System.Collections.Generic;

namespace Vstk.Flow
{
    internal class ContextProperty
    {
        public ContextProperty(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }
        public object Value { get; }

        public KeyValuePair<string, object> ToKeyValuePair()
        {
            return new KeyValuePair<string, object>(Key, Value);
        }
    }
}
