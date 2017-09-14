namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common
{
	/// <summary>
	/// <para>Отличается от <see cref="System.Net.HttpStatusCode"/> новыми кодами для неудачи при коннекте/пересылке данных, таймауте, отмене запроса (450-454).</para>
	/// <para>В остальном список соответствует http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html. </para>
	/// </summary>
	public enum HttpResponseCode
	{
		Unknown = 0,

		// (iloktionov): Informational 1xx
		Continue = 100,
		SwitchingProtocols = 101,

		// (iloktionov): Successful 2xx
		Ok = 200,
		Created = 201,
		Accepted = 202,
		NonAuthoritativeInformation = 203,
		NoContent = 204,
		ResetContent = 205,
		PartialContent = 206,

		// (iloktionov): Redirection 3xx
		MultipleChoices = 300,
		MovedPermanently = 301,
		Found = 302,
		SeeOther = 303,
        NotModified = 304,
        UseProxy = 305,
        TemporaryRedirect = 307,

		// (iloktionov): Client error 4xx
		BadRequest = 400,
		Unauthorized = 401,
		PaymentRequired = 402,
		Forbidden = 403,
		NotFound = 404,
		MethodNotAllowed = 405,
		NotAcceptable = 406,
		ProxyAuthenticationRequired = 407,
		/// <summary>
		/// Означает, что клиенту не удалось получить ответ от сервера за выделенное время.
		/// </summary>
		RequestTimeout = 408,
		Conflict = 409,
		Gone = 410,
		LengthRequired = 411,
		PreconditionFailed = 412,
		RequestEntityTooLarge = 413,
		RequestURITooLong = 414,
		UnsupportedMediaType = 415,
		RequestedRangeNotSatisfiable = 416,
		ExpectationFailed = 417,
        TooManyRequests = 429,
		/// <summary>
		/// Означает, что клиенту не удалось установить соединение с сервером.
		/// </summary>
		ConnectFailure = 450,
		/// <summary>
		/// Означет, что во время приема данных с сервера произошла ошибка.
		/// </summary>
		ReceiveFailure = 451,
		/// <summary>
		/// Означает, что во время передачи данных на сервер произошла ошибка.
		/// </summary>
		SendFailure = 452,
		/// <summary>
		/// Означает, что во время выполнения запроса произошла неизвестная ошибка на стороне клиента.
		/// </summary>
		UnknownFailure = 453,
        /// <summary>
        /// Означает, что запрос был отменён.
        /// </summary>
        Canceled = 454,

		// (iloktionov): Server error 5xx
		InternalServerError = 500,
		NotImplemented = 501,
		BadGateway = 502,
		ServiceUnavailable = 503,
		ProxyTimeout = 504,
		HttpVersionNotSupported = 505,
	}
}