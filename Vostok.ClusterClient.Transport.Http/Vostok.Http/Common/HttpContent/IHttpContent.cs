using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.HttpContent
{
	/// <summary>
	/// Представляет контент, готовый к передаче от клиента к серверу или от сервера к клиенту в рамках HTTP-запроса.
	/// </summary>
	public interface IHttpContent
	{
		void CopyTo(Stream outputStream);
		Task CopyToAsync(Stream outputStream);

		/// <summary>
		/// Длина содержимого в байтах.
		/// </summary>
		long Length { get; }

		/// <summary>
		/// MIME-тип контента.
		/// </summary>
		ContentType ContentType { get; set; }
		
		/// <summary>
		/// Кодировка, в соответствии с коротой нужно интерпретировать контент. По умолчанию UTF-8.
		/// </summary>
		Encoding Charset { get; set; }

		/// <summary>
		/// При частичной передаче контента определяет диапазон в целом контенте. В противном случае равен null.
		/// </summary>
		ContentRangeHeaderValue ContentRange { get; set; }
	}
}
