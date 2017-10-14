namespace Vostok.Clusterclient.Model
{
    /// <summary>
    /// <para>Represents an http response code. All members have correct numeric values.</para>
    /// <para>Values are taken from corresponding RFC (http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html) with several custom extensions:</para>
    /// <list type="bullet">
    /// <item><see cref="ConnectFailure"/></item>
    /// <item><see cref="ReceiveFailure"/></item>
    /// <item><see cref="SendFailure"/></item>
    /// <item><see cref="UnknownFailure"/></item>
    /// <item><see cref="Unknown"/></item>
    /// <item><see cref="Canceled"/></item>
    /// </list>
    /// </summary>
    public enum ResponseCode
    {
        /// <summary>
        /// Represents a lack of actual response or a completely unknown response code.
        /// </summary>
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

        // (iloktionov): Network error (extensions) 4xx
        /// <summary>
        /// Represents a failure in establishing/using a TCP connection.
        /// </summary>
        ConnectFailure = 450,

        /// <summary>
        /// Represents a failure during transmission of response data from server.
        /// </summary>
        ReceiveFailure = 451,

        /// <summary>
        /// Represents a failure during transmission of request data to server.
        /// </summary>
        SendFailure = 452,

        /// <summary>
        /// Represents a generic failure of unknown nature.
        /// </summary>
        UnknownFailure = 453,

        /// <summary>
        /// Represents intentional cancellation of request.
        /// </summary>
        Canceled = 454,

        // (iloktionov): Server error 5xx
        InternalServerError = 500,
        NotImplemented = 501,
        BadGateway = 502,
        ServiceUnavailable = 503,
        ProxyTimeout = 504,
        HttpVersionNotSupported = 505
    }
}
