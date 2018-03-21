using System;
using System.Globalization;

namespace Vstk.Commons.Utilities
{
    public static class DoubleParser
    {
        public static double ParseDouble(string input)
        {
            if (double.TryParse(input, NumberStyles.Float, new NumberFormatInfo { NumberDecimalSeparator = "," }, out var result))
                return result;
            if (double.TryParse(input, NumberStyles.Float, new NumberFormatInfo { NumberDecimalSeparator = "." }, out result))
                return result;
            throw new FormatException($"Error in parsing string {input} to Double.");
        }
    }
}