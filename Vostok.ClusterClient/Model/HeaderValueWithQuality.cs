using System;
using System.Globalization;
using System.Linq;

namespace Vostok.Clusterclient.Model
{
    public class HeaderValueWithQuality
    {
        public HeaderValueWithQuality(string value, decimal quality = 1m)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be empty", nameof(value));

            if (value.Contains(',') || value.Contains(';'))
                throw new ArgumentException("Value cannot contain delimiters", nameof(value));

            if (quality > 1m || quality < 0m)
                throw new ArgumentException("Quality must be in range [0; 1]", nameof(quality));

            Value = value;
            Quality = quality;
        }

        public string Value { get; }
        public decimal Quality { get; }

        public override string ToString()
        {
            return Quality < 1m ? $"{Value};q={Quality.ToString("0.###", NumberFormatInfo.InvariantInfo)}" : Value;
        }

        #region Equality members 

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is HeaderValueWithQuality && Equals((HeaderValueWithQuality) obj);
        }

        public bool Equals(HeaderValueWithQuality other)
        {
            return string.Equals(Value, other.Value, StringComparison.InvariantCultureIgnoreCase) && Quality == other.Quality;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Value?.GetHashCode() ?? 0)*397) ^ Quality.GetHashCode();
            }
        }

        #endregion
    }
}
