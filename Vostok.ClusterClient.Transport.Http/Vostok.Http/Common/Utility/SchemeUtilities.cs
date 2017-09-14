using System;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility
{
    public static class SchemeUtilities
	{
		public static bool UriHasCorrectScheme(Uri uri)
		{
			return uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
				|| uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);
		}

		public static string ToLowercaseString(HttpScheme scheme)
		{
			return scheme == HttpScheme.Https ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
		}
	}
}