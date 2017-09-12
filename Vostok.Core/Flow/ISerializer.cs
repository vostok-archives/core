using System;

namespace Vostok.Flow
{
    public interface ISerializer
    {
        bool TrySerialize(object obj, out string result);
        bool TryDeserialize(string str, Type type, out object result);
    }
}