using System;
using Vstk.Commons.Model;

namespace Vstk.Commons.Utilities
{
    internal static class DataSizeParser
    {
        private const string bytes1 = "b";
        private const string bytes2 = "bytes";

        private const string kilobytes1 = "kb";
        private const string kilobytes2 = "kilobytes";

        private const string megabytes1 = "mb";
        private const string megabytes2 = "megabytes";

        private const string gigabytes1 = "gb";
        private const string gigabytes2 = "gigabytes";

        private const string terabytes1 = "tb";
        private const string terabytes2 = "terabytes";

        private const string petabytes1 = "pb";
        private const string petabytes2 = "petabytes";

        public static DataSize Parse(string input)
        {
            input = input.ToLower();

            if (input.Contains(petabytes2))
                return DataSize.FromPetabytes(DoubleParser.ParseDouble(PrepareInput(input, petabytes2)));
            if (input.Contains(terabytes2))
                return DataSize.FromTerabytes(DoubleParser.ParseDouble(PrepareInput(input, terabytes2)));
            if (input.Contains(gigabytes2))
                return DataSize.FromGigabytes(DoubleParser.ParseDouble(PrepareInput(input, gigabytes2)));
            if (input.Contains(megabytes2))
                return DataSize.FromMegabytes(DoubleParser.ParseDouble(PrepareInput(input, megabytes2)));
            if (input.Contains(kilobytes2))
                return DataSize.FromKilobytes(DoubleParser.ParseDouble(PrepareInput(input, kilobytes2)));
            if (input.Contains(bytes2))
                return DataSize.FromBytes(long.Parse(PrepareInput(input, bytes2)));

            if (input.Contains(petabytes1))
                return DataSize.FromPetabytes(DoubleParser.ParseDouble(PrepareInput(input, petabytes1)));
            if (input.Contains(terabytes1))
                return DataSize.FromTerabytes(DoubleParser.ParseDouble(PrepareInput(input, terabytes1)));
            if (input.Contains(gigabytes1))
                return DataSize.FromGigabytes(DoubleParser.ParseDouble(PrepareInput(input, gigabytes1)));
            if (input.Contains(megabytes1))
                return DataSize.FromMegabytes(DoubleParser.ParseDouble(PrepareInput(input, megabytes1)));
            if (input.Contains(kilobytes1))
                return DataSize.FromKilobytes(DoubleParser.ParseDouble(PrepareInput(input, kilobytes1)));
            if (input.Contains(bytes1))
                return DataSize.FromBytes(long.Parse(PrepareInput(input, bytes1)));

            if (long.TryParse(input, out var bytes))
                return DataSize.FromBytes(bytes);

            throw new FormatException($"DataSizeParser. Failed to parse DataSize from string '{input}'.");
        }

        private static string PrepareInput(string input, string unit)
        {
            return input
                .Replace(unit, string.Empty)
                .Trim('.')
                .Trim();
        }
    }
}