namespace Vostok.Clusterclient.Model
{
    /// <summary>
    /// <para>Contains the names of well-known common HTTP headers.</para>
    /// <para>Values are taken from corresponding RFC (https://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html).</para>
    /// </summary>
    public static class HeaderNames
    {
        public const string Accept = "Accept";
        public const string AcceptCharset = "Accept-Charset";
        public const string AcceptEncoding = "Accept-Encoding";
        public const string AcceptLanguage = "Accept-Language";
        public const string AccessControlRequestMethod = "Access-Control-Request-Method";
        public const string AccessControlRequestHeader = "Access-Control-Request-Headers";
        public const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        public const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
        public const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";
        public const string AccessControlMaxAge = "Access-Control-Max-Age";
        public const string AccessControlAllowCredentials = "Access-Control-Allow-Credentials";
        public const string Age = "Age";
        public const string Allow = "Allow";
        public const string Authorization = "Authorization";
        public const string CacheControl = "Cache-Control";
        public const string Cookie = "Cookie";
        public const string Connection = "Connection";
        public const string ContentEncoding = "Content-Encoding";
        public const string ContentLength = "Content-Length";
        public const string ContentType = "Content-Type";
        public const string ContentLanguage = "Content-Language";
        public const string ContentLocation = "Content-Location";
        public const string ContentDisposition = "Content-Disposition";
        public const string ContentRange = "Content-Range";
        public const string ContentMD5 = "Content-MD5";
        public const string Date = "Date";
        public const string ETag = "ETag";
        public const string Expect = "Expect";
        public const string Expires = "Expires";
        public const string From = "From";
        public const string Host = "Host";
        public const string IfMatch = "If-Match";
        public const string IfModifiedSince = "If-Modified-Since";
        public const string IfNoneMatch = "If-None-Match";
        public const string IfRange = "If-Range";
        public const string IfUnmodifiedSince = "If-Unmodified-Since";
        public const string KeepAlive = "Keep-Alive";
        public const string LastModified = "Last-Modified";
        public const string Location = "Location";
        public const string Pragma = "Pragma";
        public const string ProxyConnection = "Proxy-Connection";
        public const string ProxyAuthenticate = "Proxy-Authenticate";
        public const string Range = "Range";
        public const string Referer = "Referer";
        public const string RetryAfter = "Retry-After";
        public const string Server = "Server";
        public const string SetCookie = "Set-Cookie";
        public const string TE = "TE";
        public const string Trailer = "Trailer";
        public const string TransferEncoding = "Transfer-Encoding";
        public const string Upgrade = "Upgrade";
        public const string UserAgent = "User-Agent";
        public const string Warning = "Warning";
        public const string WWWAuthenticate = "WWW-Authenticate";
        public const string Via = "Via";

        // todo (spaceorc, 21.10.2017) what to do with this kontur-specific headers
        public const string XKonturRequestTimeout = "X-Kontur-Request-Timeout";
        public const string XKonturRequestPriority = "X-Kontur-Request-Priority";
        public const string XKonturClientIdentity = "X-Kontur-Client-Identity";
        public const string XKonturDontRetry = "X-Kontur-Dont-Retry";
        public const string XDistributedContextPrefix = "X-Distributed-Context";
    }
}
