using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.HttpContent
{
	public abstract class HttpContentBase : IHttpContent
	{
		public abstract void CopyTo(Stream outputStream);

		public abstract Task CopyToAsync(Stream outputStream);
		
		public abstract long Length { get; }

		public ContentType ContentType { get; set; }

		public Encoding Charset { get; set; }

		public ContentRangeHeaderValue ContentRange { get; set; }
	}
}