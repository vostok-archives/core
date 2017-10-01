using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Client
{
	/// <summary>
	/// Необходимо использовать <see cref="HttpClientSettingsBuilder"/>, чтобы создать опции, отличные от опций по умолчанию.
	/// </summary>
	public class HttpClientSettings
	{
		internal HttpClientSettings()
		{
			ConnectionAttempts = 1;
			MaxConnectionsPerEndpoint = 10 * 1000;
			UseNagleAlgorithm = false;
			AllowAutoRedirect = true;
			Proxy = null;
			ClientCertificates = new List<X509Certificate2>();
			KeepAlive = true;
			UseConnectTimeout = false;
		    ConnectTimeout = TimeSpan.FromSeconds(1);
		}

		/// <summary>
		/// Количество попыток соединения с сервером, которые предпримет клиент в случае неудачи.
		/// </summary>
		public int ConnectionAttempts { get; internal set; }
		
		/// <summary>
		/// Максимальное количество одновременных соединений с одним EndPoint'ом (host + port).
		/// </summary>
		public int MaxConnectionsPerEndpoint { get; internal set; }

		/// <summary>
		/// Использовать ли алгоритм Нагла. Не рекомендуется включать для "легких" и редких запросов. По умолчанию выключен.
		/// </summary>
		public bool UseNagleAlgorithm { get; internal set; }

		/// <summary>
		/// Разрешает автоматическую переадресацию. По умолчанию разрешено.
		/// </summary>
		public bool AllowAutoRedirect { get; internal set; }
		
		/// <summary>
		/// Прокси-сервер для запросов. По умолчанию отсутствует.
		/// </summary>
		public IWebProxy Proxy { get; internal set; }

		/// <summary>
		/// Список ssl-сертификатор. По умолчанию пуст.
		/// </summary>
		public List<X509Certificate2> ClientCertificates { get; internal set; }

		/// <summary>
		/// Включить/выключить keep-alive
		/// </summary>
		public bool KeepAlive { get; internal set; }

		/// <summary>
		/// Отправлять ли данные для аутентификации через Active Directory
		/// </summary>
		public bool SendDomainIdentity { get; internal set; }

		/// <summary>
		/// Какие данные отправлять для аутентификации. Если DomainIdentity == null 
		/// и SendDomainIdentity == true, используются данные текущего пользователя
		/// </summary>
		public NetworkCredential DomainIdentity { get; internal set; }

		/// <summary>
		/// Ограничивать ли время на подключение к серверу
		/// </summary>
		public bool UseConnectTimeout { get; internal set; }

		/// <summary>
		/// Таймаут на подключение к серверу. Используется только если UseConnectTimeout == true
		/// </summary>
		public TimeSpan ConnectTimeout { get; internal set; }
	}
}