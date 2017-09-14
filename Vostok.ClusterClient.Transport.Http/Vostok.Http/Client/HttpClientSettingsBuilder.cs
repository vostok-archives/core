using System;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Client
{
	public class HttpClientSettingsBuilder
	{
		public HttpClientSettingsBuilder()
		{
			settings = new HttpClientSettings();
		}

		public static HttpClientSettingsBuilder StartNew()
		{
			return new HttpClientSettingsBuilder();
		}

		public HttpClientSettings Build()
		{
			return settings;
		}

		public HttpClientSettingsBuilder SetConnectionAttempts(int connectionAttempts)
		{
			Preconditions.EnsureArgumentRange(connectionAttempts > 0, "connectionAttempts", "Connection attempts count must be > 0.");
			settings.ConnectionAttempts = connectionAttempts;
			return this;
		}

		public HttpClientSettingsBuilder EnableConnectTimeout(TimeSpan timeout)
		{
			settings.UseConnectTimeout = true;
			settings.ConnectTimeout = timeout;
			return this;
		}

        private readonly HttpClientSettings settings;
	}
}