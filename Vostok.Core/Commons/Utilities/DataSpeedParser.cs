using System;
using Vstk.Commons.Model;

namespace Vstk.Commons.Utilities
{
    internal static class DataSpeedParser
    {
        private const string second1 = "/sec";
        private const string second2 = "/second";

        public static DataSpeed Parse(string input)
        {
            input = input
                .ToLower()
                .Replace(second2, string.Empty)
                .Replace(second1, string.Empty)
                .Trim('.')
                .Trim();

            return DataSizeParser.Parse(input) / TimeSpan.FromSeconds(1);
        }
    }
}