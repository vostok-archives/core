using System;
using Vostok.Commons.Model;

namespace Vostok.Commons.Utilities
{
    internal static class DataSizeParser
    {
        private const string Bytes1 = "b";
        private const string Bytes2 = "bytes";

        private const string Kilobytes1 = "kb";
        private const string Kilobytes2 = "kilobytes";

        private const string Megabytes1 = "mb";
        private const string Megabytes2 = "megabytes";

        private const string Gigabytes1 = "gb";
        private const string Gigabytes2 = "gigabytes";

        private const string Terabytes1 = "tb";
        private const string Terabytes2 = "terabytes";

        private const string Petabytes1 = "pb";
        private const string Petabytes2 = "petabytes";

        public static DataSize Parse(string input)
        {
            input = input.ToLower();

            if (input.Contains(Petabytes2))
                return DataSize.FromPetabytes(DoubleParser.ParseDouble(PrepareInput(input, Petabytes2)));
            if (input.Contains(Terabytes2))
                return DataSize.FromTerabytes(DoubleParser.ParseDouble(PrepareInput(input, Terabytes2)));
            if (input.Contains(Gigabytes2))
                return DataSize.FromGigabytes(DoubleParser.ParseDouble(PrepareInput(input, Gigabytes2)));
            if (input.Contains(Megabytes2))
                return DataSize.FromMegabytes(DoubleParser.ParseDouble(PrepareInput(input, Megabytes2)));
            if (input.Contains(Kilobytes2))
                return DataSize.FromKilobytes(DoubleParser.ParseDouble(PrepareInput(input, Kilobytes2)));
            if (input.Contains(Bytes2))
                return DataSize.FromBytes(long.Parse(PrepareInput(input, Bytes2)));

            if (input.Contains(Petabytes1))
                return DataSize.FromPetabytes(DoubleParser.ParseDouble(PrepareInput(input, Petabytes1)));
            if (input.Contains(Terabytes1))
                return DataSize.FromTerabytes(DoubleParser.ParseDouble(PrepareInput(input, Terabytes1)));
            if (input.Contains(Gigabytes1))
                return DataSize.FromGigabytes(DoubleParser.ParseDouble(PrepareInput(input, Gigabytes1)));
            if (input.Contains(Megabytes1))
                return DataSize.FromMegabytes(DoubleParser.ParseDouble(PrepareInput(input, Megabytes1)));
            if (input.Contains(Kilobytes1))
                return DataSize.FromKilobytes(DoubleParser.ParseDouble(PrepareInput(input, Kilobytes1)));
            if (input.Contains(Bytes1))
                return DataSize.FromBytes(long.Parse(PrepareInput(input, Bytes1)));

            long bytes;

            if (long.TryParse(input, out bytes))
                return DataSize.FromBytes(bytes);

            throw new FormatException(string.Format("DataSizeParser. Failed to parse DataSize from string '{0}'.", input));
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