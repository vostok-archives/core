using System.Text;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.HttpContent
{
	public class StringContent : ByteArrayContent
	{
		public StringContent(string content, Encoding charset, ContentType contentType)
			: base(charset.GetBytes(content))
		{
			originalString = content;
			ContentType = contentType;
			Charset = charset;
		}

		public StringContent(string content, Encoding charset)
			: this (content, charset, ContentType.PlainText) { }

		public StringContent(string content, ContentType contentType)
			: this (content, EncodingFactory.GetDefault(), contentType) { }

		public StringContent(string content )
			: this (content, ContentType.PlainText) { }

		public override string ToString()
		{
			return originalString;
		}

		public string OriginalString
		{
			get { return originalString; }
		}

		private readonly string originalString;
	}
}