using System;
using System.Net.Http.Headers;
using System.Text;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Headers
{
    public class BasicAuthorizationHeaderValue : AuthenticationHeaderValue
	{
		public BasicAuthorizationHeaderValue(string userName, string password, Encoding encoding)
			: base ("Basic", EncodeCredentials(userName, password, encoding)) { }

		public BasicAuthorizationHeaderValue(string userName, string password)
			: this(userName, password, EncodingFactory.GetDefault()) { }

		private static string EncodeCredentials(string userName, string password, Encoding encoding)
		{
			Preconditions.EnsureNotNull(userName, "userName");
			Preconditions.EnsureNotNull(password, "password");
			return Convert.ToBase64String(encoding.GetBytes(userName + ":" + password));
		}
	}
}