using System;

namespace Vostok.Commons.Extensions.Uri
{
    public static class UriFormattingExtensions
    {
        public static string ToStringWithoutQuery(this System.Uri url)
        {
            var urlString = url.ToString();

            var queryBeginning = urlString.IndexOf("?", StringComparison.Ordinal);
            if (queryBeginning >= 0)
                urlString = urlString.Substring(0, queryBeginning);

            return urlString;
        }
    }
}
