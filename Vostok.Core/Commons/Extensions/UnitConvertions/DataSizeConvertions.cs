using Vostok.Commons.Model;

namespace Vostok.Commons.Extensions.UnitConvertions
{
    public static class DataSizeConvertions
    {
        public static DataSize Bytes(this ushort value)
        {
            return DataSize.FromBytes(value);
        }

        public static DataSize Bytes(this int value)
        {
            return DataSize.FromBytes(value);
        }

        public static DataSize Bytes(this long value)
        {
            return DataSize.FromBytes(value);
        }

        public static DataSize Kilobytes(this ushort value)
        {
            return DataSize.FromKilobytes(value);
        }

        public static DataSize Kilobytes(this int value)
        {
            return DataSize.FromKilobytes(value);
        }

        public static DataSize Kilobytes(this long value)
        {
            return DataSize.FromKilobytes(value);
        }

        public static DataSize Kilobytes(this double value)
        {
            return DataSize.FromKilobytes(value);
        }

        public static DataSize Megabytes(this ushort value)
        {
            return DataSize.FromMegabytes(value);
        }

        public static DataSize Megabytes(this int value)
        {
            return DataSize.FromMegabytes(value);
        }

        public static DataSize Megabytes(this long value)
        {
            return DataSize.FromMegabytes(value);
        }

        public static DataSize Megabytes(this double value)
        {
            return DataSize.FromMegabytes(value);
        }

        public static DataSize Gigabytes(this ushort value)
        {
            return DataSize.FromGigabytes(value);
        }

        public static DataSize Gigabytes(this int value)
        {
            return DataSize.FromGigabytes(value);
        }

        public static DataSize Gigabytes(this long value)
        {
            return DataSize.FromGigabytes(value);
        }

        public static DataSize Gigabytes(this double value)
        {
            return DataSize.FromGigabytes(value);
        }

        public static DataSize Terabytes(this ushort value)
        {
            return DataSize.FromTerabytes(value);
        }

        public static DataSize Terabytes(this int value)
        {
            return DataSize.FromTerabytes(value);
        }

        public static DataSize Terabytes(this long value)
        {
            return DataSize.FromTerabytes(value);
        }

        public static DataSize Terabytes(this double value)
        {
            return DataSize.FromTerabytes(value);
        }

        public static DataSize Petabytes(this ushort value)
        {
            return DataSize.FromPetabytes(value);
        }

        public static DataSize Petabytes(this int value)
        {
            return DataSize.FromPetabytes(value);
        }

        public static DataSize Petabytes(this long value)
        {
            return DataSize.FromPetabytes(value);
        }

        public static DataSize Petabytes(this double value)
        {
            return DataSize.FromPetabytes(value);
        }
    }
}
