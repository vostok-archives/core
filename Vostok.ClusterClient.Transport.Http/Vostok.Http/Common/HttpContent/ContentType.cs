using System;
using System.Text;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.HttpContent
{
	public class ContentType
	{
		public ContentType(string type)
		{
			Preconditions.EnsureNotNull(type, "type");
			Preconditions.EnsureCondition(!type.Contains(";"), "type", "Types with charset are not allowed. Use Charset property of IHttpContent.");
			Type = type;
		}

		public override string ToString()
		{
			return Type;
		}

		public static ContentType Parse(string contentTypeHeader)
		{
			var separatorIndex = contentTypeHeader.IndexOf(';');
			if (separatorIndex < 0)
				return new ContentType(contentTypeHeader);
			return new ContentType(contentTypeHeader.Substring(0, separatorIndex));
		}

		public static ContentType Parse(string contentTypeHeader, out Encoding charset)
		{
			var charsetName = HeadersParseUtility.GetAttributeFromHeader(contentTypeHeader, "charset");
			if (charsetName == null)
				charset = EncodingFactory.GetDefault();
			else
				try
				{
					charset = Encoding.GetEncoding(charsetName);
				}
				catch (Exception)
				{
					charset = EncodingFactory.GetDefault();
				}
			return Parse(contentTypeHeader);
		}

		public string Type { get; }

		#region Equality members
		protected bool Equals(ContentType other)
		{
			return string.Equals(Type, other.Type);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((ContentType)obj);
		}

		public override int GetHashCode()
		{
			return (Type != null ? Type.GetHashCode() : 0);
		} 
		#endregion

		public static readonly ContentType OctetStream = new ContentType("application/octet-stream");

		public static readonly ContentType PlainText = new ContentType("text/plain");
	}
}