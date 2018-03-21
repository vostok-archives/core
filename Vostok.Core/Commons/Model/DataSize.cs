using System;
using Vstk.Commons.Utilities;

namespace Vstk.Commons.Model
{
    [Serializable]
    public struct DataSize : IEquatable<DataSize>, IComparable<DataSize>
    {
        public static DataSize FromBytes(long bytes)
        {
            return new DataSize(bytes);
        }

        public static DataSize FromKilobytes(double kilobytes)
        {
            return new DataSize((long)(kilobytes * DataSizeConstants.Kilobyte));
        }

        public static DataSize FromMegabytes(double megabytes)
        {
            return new DataSize((long)(megabytes * DataSizeConstants.Megabyte));
        }

        public static DataSize FromGigabytes(double gigabytes)
        {
            return new DataSize((long)(gigabytes * DataSizeConstants.Gigabyte));
        }

        public static DataSize FromTerabytes(double terabytes)
        {
            return new DataSize((long)(terabytes * DataSizeConstants.Terabyte));
        }

        public static DataSize FromPetabytes(double petabytes)
        {
            return new DataSize((long)(petabytes * DataSizeConstants.Petabyte));
        }

        public static DataSize Parse(string input)
        {
            return DataSizeParser.Parse(input);
        }

        public static bool TryParse(string input, out DataSize result)
        {
            try
            {
                result = Parse(input);
                return true;
            }
            catch
            {
                result = default(DataSize);
                return false;
            }
        }

        private readonly long bytes;

        public DataSize(long bytes)
        {
            this.bytes = bytes;
        }

        public long Bytes
        {
            get { return bytes; }
        }

        public double TotalKilobytes
        {
            get { return bytes / (double)DataSizeConstants.Kilobyte; }
        }

        public double TotalMegabytes
        {
            get { return bytes / (double)DataSizeConstants.Megabyte; }
        }

        public double TotalGigabytes
        {
            get { return bytes / (double)DataSizeConstants.Gigabyte; }
        }

        public double TotalTerabytes
        {
            get { return bytes / (double)DataSizeConstants.Terabyte; }
        }

        public double TotalPetabytes
        {
            get { return bytes / (double)DataSizeConstants.Petabyte; }
        }

        public static explicit operator long(DataSize size)
        {
            return size.bytes;
        }

        public override string ToString()
        {
            return ToString(true);
        }

        public string ToString(bool shortFormat)
        {
            if (Math.Abs(TotalPetabytes) >= 1)
                return TotalPetabytes.ToString("0.##") + ' ' + (shortFormat ? "PB" : "petabytes");

            if (Math.Abs(TotalTerabytes) >= 1)
                return TotalTerabytes.ToString("0.##") + ' ' + (shortFormat ? "TB" : "terabytes");

            if (Math.Abs(TotalGigabytes) >= 1)
                return TotalGigabytes.ToString("0.##") + ' ' + (shortFormat ? "GB" : "gigabytes");

            if (Math.Abs(TotalMegabytes) >= 1)
                return TotalMegabytes.ToString("0.##") + ' ' + (shortFormat ? "MB" : "megabytes");

            if (Math.Abs(TotalKilobytes) >= 1)
                return TotalKilobytes.ToString("0.##") + ' ' + (shortFormat ? "KB" : "kilobytes");

            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            return bytes.ToString() + ' ' + (shortFormat ? "B" : "bytes");
        }

        public static DataSize operator +(DataSize size1, DataSize size2)
        {
            return new DataSize(size1.bytes + size2.bytes);
        }

        public static DataSize operator -(DataSize size1, DataSize size2)
        {
            return new DataSize(size1.bytes - size2.bytes);
        }

        public static DataSize operator *(DataSize size, int multiplier)
        {
            return new DataSize(size.bytes * multiplier);
        }

        public static DataSize operator *(int multiplier, DataSize size)
        {
            return size * multiplier;
        }

        public static DataSize operator *(DataSize size, long multiplier)
        {
            return new DataSize(size.bytes * multiplier);
        }

        public static DataSize operator *(long multiplier, DataSize size)
        {
            return size * multiplier;
        }

        public static DataSize operator *(DataSize size, double multiplier)
        {
            return new DataSize((long)(size.bytes * multiplier));
        }

        public static DataSize operator *(double multiplier, DataSize size)
        {
            return size * multiplier;
        }

        public static DataSize operator /(DataSize size, int multiplier)
        {
            return new DataSize(size.bytes / multiplier);
        }

        public static DataSize operator /(DataSize size, long multiplier)
        {
            return new DataSize(size.bytes / multiplier);
        }

        public static DataSize operator /(DataSize size, double multiplier)
        {
            return new DataSize((long)(size.bytes / multiplier));
        }

        public static DataSpeed operator /(DataSize size, TimeSpan time)
        {
            return new DataSpeed((size / time.TotalSeconds).Bytes);
        }

        public static TimeSpan operator /(DataSize size, DataSpeed speed)
        {
            return TimeSpan.FromSeconds(size.Bytes / (double)speed.BytesPerSecond);
        }

        public static DataSize operator -(DataSize size)
        {
            return new DataSize(-size.bytes);
        }

        public bool Equals(DataSize other)
        {
            return bytes == other.bytes;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is DataSize && Equals((DataSize)obj);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            return bytes.GetHashCode();
        }

        public static bool operator ==(DataSize left, DataSize right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DataSize left, DataSize right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(DataSize other)
        {
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            return bytes.CompareTo(other.bytes);
        }

        public static bool operator >(DataSize size1, DataSize size2)
        {
            return size1.bytes > size2.bytes;
        }

        public static bool operator >=(DataSize size1, DataSize size2)
        {
            return size1.bytes >= size2.bytes;
        }

        public static bool operator <(DataSize size1, DataSize size2)
        {
            return size1.bytes < size2.bytes;
        }

        public static bool operator <=(DataSize size1, DataSize size2)
        {
            return size1.bytes <= size2.bytes;
        }
    }
}
