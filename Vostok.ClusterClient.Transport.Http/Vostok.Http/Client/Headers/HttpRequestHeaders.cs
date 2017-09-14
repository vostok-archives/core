using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Headers;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.ToCore.Utilities;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Headers
{
    public class HttpRequestHeaders : WritableHeadersCollection, IEnumerable<HttpRequestHeader>
    {
        internal static readonly HashSet<string> RestrictedHeaderNames = new HashSet<string>
        {
            HttpHeaderNames.Accept,
            HttpHeaderNames.AcceptCharset,
            HttpHeaderNames.Authorization,
            HttpHeaderNames.ContentLength,
            HttpHeaderNames.ContentType,
            HttpHeaderNames.ContentRange,
            HttpHeaderNames.Host,
            HttpHeaderNames.IfMatch,
            HttpHeaderNames.IfModifiedSince,
            HttpHeaderNames.Range,
            HttpHeaderNames.Referer,
            HttpHeaderNames.UserAgent,
            HttpHeaderNames.XKonturRequestTimeout,
        };

        public static bool IsCorrectCustomHeaderName(string headerName)
        {
            if (string.IsNullOrWhiteSpace(headerName))
                return false;
            return !RestrictedHeaderNames.Contains(headerName);
        }

        public bool IsEmpty
	    {
	        get
	        {
	            if (CustomHeaders != null && CustomHeaders.Count > 0)
	                return false;

	            return string.IsNullOrEmpty(Accept) &&
                       string.IsNullOrEmpty(Host) &&
                       AcceptCharset == null &&
	                   Authorization == null &&
	                   IfMatch == null &&
	                   IfModifiedSince == null &&
	                   Range == null && 
                       string.IsNullOrEmpty(Referer) &&
	                   string.IsNullOrEmpty(UserAgent);
	        }
	    }

	    public string Accept { get; set; }

		public Encoding AcceptCharset { get; set; }

		public AuthenticationHeaderValue Authorization { get; set; }

		public string Host { get; set; }

		public EntityTagHeaderValue IfMatch { get; set; }

		public DateTime? IfModifiedSince { get; set; }

		public RangeHeaderValue Range { get; set; }

		public string Referer { get; set; }

		public string UserAgent { get; set; }

        public new string this[string headerName]
        {
            get
            {
                if (headerName.IgnoreCaseEquals(HttpHeaderNames.Accept))
                {
                    if (!string.IsNullOrWhiteSpace(Accept))
                        return Accept;
                }
                else if (headerName.IgnoreCaseEquals(HttpHeaderNames.AcceptCharset))
                {
                    if (AcceptCharset != null)
                        return AcceptCharset.WebName;
                }
                else if (headerName.IgnoreCaseEquals(HttpHeaderNames.Authorization))
                {
                    if (Authorization != null)
                        return Authorization.ToString();
                }
                else if (headerName.IgnoreCaseEquals(HttpHeaderNames.Host))
                {
                    if (!string.IsNullOrWhiteSpace(Host))
                        return Host;
                }
                else if (headerName.IgnoreCaseEquals(HttpHeaderNames.IfMatch))
                {
                    if (IfMatch != null)
                        return IfMatch.ToString();
                }
                else if (headerName.IgnoreCaseEquals(HttpHeaderNames.IfModifiedSince))
                {
                    if (IfModifiedSince.HasValue)
                        return IfModifiedSince.Value.ToHeaderStringValue();
                }
                else if (headerName.IgnoreCaseEquals(HttpHeaderNames.Range))
                {
                    if (Range != null)
                        return Range.ToHeaderStringValue();
                }
                else if (headerName.IgnoreCaseEquals(HttpHeaderNames.Referer))
                {
                    if (!string.IsNullOrWhiteSpace(Referer))
                        return Referer;
                }
                else if (headerName.IgnoreCaseEquals(HttpHeaderNames.UserAgent))
                {
                    if (!string.IsNullOrWhiteSpace(UserAgent))
                        return UserAgent;
                }
                else
                {
                    return GetCustomHeader(headerName);
                }
                return null;
            }
        }

        public string GetCustomHeader(string headerName)
        {
            return base[headerName];
        }

        protected override HashSet<string> GetRestrictedHeaderNames()
		{
			return RestrictedHeaderNames;
		}

        public IEnumerator<HttpRequestHeader> GetEnumerator()
        {
            if (!string.IsNullOrWhiteSpace(Accept))
                yield return new HttpRequestHeader("Accept", Accept);
            if (AcceptCharset != null)
                yield return new HttpRequestHeader("Accept-Charset", AcceptCharset.WebName);
            if (Authorization != null)
                yield return new HttpRequestHeader("Authorization", Authorization.ToString());
            if (!string.IsNullOrWhiteSpace(Host))
                yield return new HttpRequestHeader("Host", Host);
            if (IfMatch != null)
                yield return new HttpRequestHeader("If-Match", IfMatch.ToString());
            if (IfModifiedSince.HasValue)
                yield return new HttpRequestHeader("If-Modified-Since", IfModifiedSince.Value.ToHeaderStringValue());
            if (Range != null)
                yield return new HttpRequestHeader("Range", Range.ToHeaderStringValue());
            if (!string.IsNullOrWhiteSpace(Referer))
                yield return new HttpRequestHeader("Referer", Referer);
            if (!string.IsNullOrWhiteSpace(UserAgent))
                yield return new HttpRequestHeader("User-Agent", UserAgent);

            if (CustomHeaders != null)
            {
                foreach (var pair in CustomHeaders)
                {
                    yield return new HttpRequestHeader(pair.Key, pair.Value);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(HttpRequestHeaders other)
        {
            if (ReferenceEquals(null, other))
                return false;

            return string.Equals(Accept, other.Accept) && Equals(AcceptCharset, other.AcceptCharset) &&
                   Equals(Authorization, other.Authorization) && string.Equals(Host, other.Host) &&
                   Equals(IfMatch, other.IfMatch) && IfModifiedSince.Equals(other.IfModifiedSince) &&
                   Equals(Range, other.Range) && string.Equals(Referer, other.Referer) &&
                   string.Equals(UserAgent, other.UserAgent) &&
                   CustomHeaders.ElementwiseEquals(other.CustomHeaders);
        }

        public override bool Equals(object obj)
	    {
	        if (ReferenceEquals(null, obj)) return false;
	        if (ReferenceEquals(this, obj)) return true;
	        if (obj.GetType() != this.GetType()) return false;
	        return Equals((HttpRequestHeaders) obj);
	    }

        public override int GetHashCode()
	    {
	        unchecked
	        {
	            var hashCode = (Accept != null ? Accept.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ (AcceptCharset != null ? AcceptCharset.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ (Authorization != null ? Authorization.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ (Host != null ? Host.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ (IfMatch != null ? IfMatch.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ IfModifiedSince.GetHashCode();
	            hashCode = (hashCode*397) ^ (Range != null ? Range.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ (Referer != null ? Referer.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ (UserAgent != null ? UserAgent.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ (CustomHeaders != null ? CustomHeaders.ElementwiseHash() : 0);
	            return hashCode;
	        }
	    }

        public static bool operator ==(HttpRequestHeaders left, HttpRequestHeaders right)
	    {
	        return Equals(left, right);
	    }

	    public static bool operator !=(HttpRequestHeaders left, HttpRequestHeaders right)
	    {
	        return !Equals(left, right);
	    }

        public HttpRequestHeaders Clone()
        {
            var clone = new HttpRequestHeaders
            {
                Accept          = Accept,
                AcceptCharset   = AcceptCharset,
                Authorization   = Authorization,
                Host            = Host,
                UserAgent       = UserAgent,
                IfMatch         = IfMatch,
                IfModifiedSince = IfModifiedSince,
                Range           = Range,
                Referer         = Referer
            };

            if (CustomHeaders != null)
            {
                foreach (var pair in CustomHeaders)
                {
                    clone.ObtainCustomHeaders();
                    clone.CustomHeaders.Add(pair);
                }
            }

            return clone;
        }
    }
}