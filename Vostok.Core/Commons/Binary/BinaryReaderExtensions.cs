using System;
using System.Collections.Generic;
using System.Text;

namespace Vostok.Commons.Binary
{
    public static class BinaryReaderExtensions
    {
        public static string ReadString(this IBinaryReader reader)
        {
            return reader.ReadString(Encoding.UTF8);
        }

        public static string ReadString(this IBinaryReader reader, int length)
        {
            return reader.ReadString(Encoding.UTF8, length);
        }

        public static TimeSpan ReadTimeSpan(this IBinaryReader reader)
        {
            return new TimeSpan(reader.ReadInt64());
        }

        public static T[] ReadArray<T>(this IBinaryReader reader, Func<IBinaryReader, T> readSingleValue)
        {
            var count = reader.ReadInt32();
            var result = new T[count];

            for (var i = 0; i < count; i++)
            {
                result[i] = readSingleValue(reader);
            }

            return result;
        }

        public static List<T> ReadList<T>(this IBinaryReader reader, Func<IBinaryReader, T> readSingleValue)
        {
            var count = reader.ReadInt32();
            var result = new List<T>(count);

            for (var i = 0; i < count; i++)
            {
                result.Add(readSingleValue(reader));
            }

            return result;
        }

        public static HashSet<T> ReadSet<T>(this IBinaryReader reader, Func<IBinaryReader, T> readSingleValue)
        {
            var count = reader.ReadInt32();
            var result = new HashSet<T>();

            for (var i = 0; i < count; i++)
            {
                result.Add(readSingleValue(reader));
            }

            return result;
        }

        public static Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>(
            this IBinaryReader reader,
            Func<IBinaryReader, TKey> readSingleKey,
            Func<IBinaryReader, TValue> readSingleValue)
        {
            var count = reader.ReadInt32();
            var result = new Dictionary<TKey, TValue>(count);

            for (var i = 0; i < count; i++)
            {
                var key = readSingleKey(reader);
                var value = readSingleValue(reader);

                result[key] = value;
            }

            return result;
        }

        public static T ReadNullable<T>(this IBinaryReader reader, Func<IBinaryReader, T> readNonNullValue)
            where T : class
        {
            return reader.ReadBool() ? readNonNullValue(reader) : null;
        }

        public static T? ReadNullableStruct<T>(this IBinaryReader reader, Func<IBinaryReader, T> readNonNullValue)
            where T : struct
        {
            return reader.ReadBool() ? readNonNullValue(reader) : (T?) null;
        }
    }
}
