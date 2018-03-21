using System;
using Vostok.Commons.Utilities;

namespace Vostok.Commons.Model
{
    [Serializable]
    public struct DataSpeed : IEquatable<DataSpeed>, IComparable<DataSpeed>
    {
        public static DataSpeed FromBytesPerSecond(long bytes)
        {
            return new DataSpeed(bytes);
        }

        public static DataSpeed FromKilobytesPerSecond(double kilobytes)
        {
            return new DataSpeed((long)(kilobytes * DataSizeConstants.Kilobyte));
        }

        public static DataSpeed FromMegabytesPerSecond(double megabytes)
        {
            return new DataSpeed((long)(megabytes * DataSizeConstants.Megabyte));
        }

        public static DataSpeed FromGigabytesPerSecond(double gigabytes)
        {
            return new DataSpeed((long)(gigabytes * DataSizeConstants.Gigabyte));
        }

        public static DataSpeed FromTerabytesPerSecond(double terabytes)
        {
            return new DataSpeed((long)(terabytes * DataSizeConstants.Terabyte));
        }

        public static DataSpeed FromPetabytesPerSecond(double petabytes)
        {
            return new DataSpeed((long)(petabytes * DataSizeConstants.Petabyte));
        }

        public static DataSpeed Parse(string input)
        {
            return DataSpeedParser.Parse(input);
        }

        public static bool TryParse(string input, out DataSpeed result)
        {
            try
            {
                result = Parse(input);
                return true;
            }
            catch
            {
                result = default(DataSpeed);
                return false;
            }
        }

        private readonly long bytesPerSecond;

        public DataSpeed(long bytesPerSecond)
        {
            this.bytesPerSecond = bytesPerSecond;
        }

        public long BytesPerSecond
        {
            get { return bytesPerSecond; }
        }

        public double KilobytesPerSecond
        {
            get { return bytesPerSecond / (double)DataSizeConstants.Kilobyte; }
        }

        public double MegabytesPerSecond
        {
            get { return bytesPerSecond / (double)DataSizeConstants.Megabyte; }
        }

        public double GigabytesPerSecond
        {
            get { return bytesPerSecond / (double)DataSizeConstants.Gigabyte; }
        }

        public double TerabytesPerSecond
        {
            get { return bytesPerSecond / (double)DataSizeConstants.Terabyte; }
        }

        public double PetabytesPerSecond
        {
            get { return bytesPerSecond / (double)DataSizeConstants.Petabyte; }
        }

        public override string ToString()
        {
            return ToString(true);
        }

        public string ToString(bool shortFormat)
        {
            if (PetabytesPerSecond >= 1)
                return PetabytesPerSecond.ToString("0.####") + ' ' + (shortFormat ? "PB/sec" : "petabytes/second");

            if (TerabytesPerSecond >= 1)
                return TerabytesPerSecond.ToString("0.####") + ' ' + (shortFormat ? "TB/sec" : "terabytes/second");

            if (GigabytesPerSecond >= 1)
                return GigabytesPerSecond.ToString("0.####") + ' ' + (shortFormat ? "GB/sec" : "gigabytes/second");

            if (MegabytesPerSecond >= 1)
                return MegabytesPerSecond.ToString("0.####") + ' ' + (shortFormat ? "MB/sec" : "megabytes/second");

            if (KilobytesPerSecond >= 1)
                return KilobytesPerSecond.ToString("0.####") + ' ' + (shortFormat ? "KB/sec" : "kilobytes/second");

            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            return bytesPerSecond.ToString() + ' ' + (shortFormat ? "B/sec" : "bytes/second");
        }

        public static DataSpeed operator +(DataSpeed speed1, DataSpeed speed2)
        {
            return new DataSpeed(speed1.bytesPerSecond + speed2.bytesPerSecond);
        }

        public static DataSpeed operator -(DataSpeed speed1, DataSpeed speed2)
        {
            return new DataSpeed(speed1.bytesPerSecond - speed2.bytesPerSecond);
        }

        public static DataSpeed operator *(DataSpeed speed, int multiplier)
        {
            return new DataSpeed(speed.bytesPerSecond * multiplier);
        }

        public static DataSpeed operator *(DataSpeed speed, long multiplier)
        {
            return new DataSpeed(speed.bytesPerSecond * multiplier);
        }

        public static DataSpeed operator *(DataSpeed speed, double multiplier)
        {
            return new DataSpeed((long)(speed.bytesPerSecond * multiplier));
        }

        public static DataSpeed operator /(DataSpeed speed, int multiplier)
        {
            return new DataSpeed(speed.bytesPerSecond / multiplier);
        }

        public static DataSpeed operator /(DataSpeed speed, long multiplier)
        {
            return new DataSpeed(speed.bytesPerSecond / multiplier);
        }

        public static DataSpeed operator /(DataSpeed speed, double multiplier)
        {
            return new DataSpeed((long)(speed.bytesPerSecond / multiplier));
        }

        public static DataSize operator *(DataSpeed speed, TimeSpan time)
        {
            return new DataSize((long)(speed.bytesPerSecond * time.TotalSeconds));
        }

        public static DataSize operator *(TimeSpan time, DataSpeed speed)
        {
            return new DataSize((long)(speed.bytesPerSecond * time.TotalSeconds));
        }

        public bool Equals(DataSpeed other)
        {
            return bytesPerSecond == other.bytesPerSecond;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is DataSpeed && Equals((DataSpeed)obj);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            return bytesPerSecond.GetHashCode();
        }

        public static bool operator ==(DataSpeed left, DataSpeed right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DataSpeed left, DataSpeed right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(DataSpeed other)
        {
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            return bytesPerSecond.CompareTo(other.bytesPerSecond);
        }
    }
}