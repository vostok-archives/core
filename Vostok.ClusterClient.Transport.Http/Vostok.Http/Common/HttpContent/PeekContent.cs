using System.IO;
using System.Threading.Tasks;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.HttpContent
{
	/// <summary>
	/// <para>При использовании этого контента заполняется значение заголовка Content-Length, но не отправляются данные.</para>
	/// <para>Единственное применение - ответ сервера на HEAD-запрос. При этом клиент может оценить длину контента, не скачивая его.</para>
	/// <para>Применение на клиентской стороне или в ответе на методы, отличные от HEAD, вызовет исключение.</para>
	/// </summary>
	public class PeekContent : HttpContentBase
	{
		public PeekContent(long length)
		{
			this.length = length;
		}

		public override void CopyTo(Stream outputStream)
		{
		}

#pragma warning disable 1998
		public override async Task CopyToAsync(Stream outputStream)
		{
		}
#pragma warning restore 1998

        public override long Length
		{
			get { return length; }
		}

		private readonly long length;
	}
}