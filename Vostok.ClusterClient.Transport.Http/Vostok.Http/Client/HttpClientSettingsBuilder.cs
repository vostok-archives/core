using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
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

		public HttpClientSettingsBuilder SetMaxConnectionsPerEndpoint(int maxConnections)
		{
			Preconditions.EnsureArgumentRange(maxConnections > 0, "maxConnections", "Max connections count must be > 0.");
			settings.MaxConnectionsPerEndpoint = maxConnections;
			return this;
		}

		public HttpClientSettingsBuilder EnableNagleAlgorithm()
		{
			settings.UseNagleAlgorithm = true;
			return this;
		}

		public HttpClientSettingsBuilder DisableAutoRedirect()
		{
			settings.AllowAutoRedirect = false;
			return this;
		}

		public HttpClientSettingsBuilder DisableKeepAlive()
		{
			settings.KeepAlive = false;
			return this;
		}

		public HttpClientSettingsBuilder SetWebProxy(IWebProxy proxy)
		{
			settings.Proxy = proxy;
			return this;
		}

		public HttpClientSettingsBuilder AddClientCertificate(X509Certificate2 certificate)
		{
			settings.ClientCertificates.Add(certificate);
			return this;
		}

	    public HttpClientSettingsBuilder SendDomainIdentity(bool send, NetworkCredential credentials = null)
	    {
	        settings.SendDomainIdentity = send;
	        settings.DomainIdentity = credentials;
	        return this;
	    }

		public HttpClientSettingsBuilder EnableConnectTimeout(TimeSpan timeout)
		{
			settings.UseConnectTimeout = true;
			settings.ConnectTimeout = timeout;
			return this;
		}

        public HttpClientSettingsBuilder DisableConnectTimeout()
        {
            settings.UseConnectTimeout = false;
            return this;
        }

        private readonly HttpClientSettings settings;
	}
}