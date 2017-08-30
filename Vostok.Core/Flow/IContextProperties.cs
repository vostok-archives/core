using System.Collections.Generic;

namespace Vostok.Flow
{
    public interface IContextProperties
    {
        IReadOnlyDictionary<string, object> Current { get; }

        void SetProperty(string key, object value);

        void RemoveProperty(string key);
    }
}
