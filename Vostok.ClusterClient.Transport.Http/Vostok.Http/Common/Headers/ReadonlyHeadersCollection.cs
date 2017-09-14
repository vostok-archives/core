using System.Collections.Specialized;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Headers
{
	/// <summary>
	/// Коллекция заголовков, которую заполняет библиотека и читает пользователь.
	/// </summary>
	public abstract class ReadonlyHeadersCollection
	{
		protected ReadonlyHeadersCollection(NameValueCollection headers)
		{
			this.headers = headers;
		}

		public override string ToString()
		{
			return headers.ToString();
		}

		public int Count => headers.Count;

	    public string[] Keys => headers.AllKeys;

	    public string this[string headerName] => headers[headerName];

	    protected readonly NameValueCollection headers;
	}
}