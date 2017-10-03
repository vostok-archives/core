using System;
using System.Globalization;

namespace Vostok.Commons.Utilities
{
    public static class DoubleParser
    {
        public static double ParseDouble(string input)
        {
            if (double.TryParse(input, NumberStyles.Float, new NumberFormatInfo { NumberDecimalSeparator = "," }, out var result))
                return result;
            if (double.TryParse(input, NumberStyles.Float, new NumberFormatInfo { NumberDecimalSeparator = "." }, out result))
                return result;
            throw new FormatException(string.Format("Error in parsing string {0} to Double.", input));
        }
    }
}