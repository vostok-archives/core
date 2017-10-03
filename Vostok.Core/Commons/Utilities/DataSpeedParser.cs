using System;
using Vostok.Commons.Model;

namespace Vostok.Commons.Utilities
{
    internal static class DataSpeedParser
    {
        private const string Second1 = "/sec";
        private const string Second2 = "/second";

        public static DataSpeed Parse(string input)
        {
            input = input
                .ToLower()
                .Replace(Second2, string.Empty)
                .Replace(Second1, string.Empty)
                .Trim('.')
                .Trim();

            return DataSizeParser.Parse(input) / TimeSpan.FromSeconds(1);
        }
    }
}