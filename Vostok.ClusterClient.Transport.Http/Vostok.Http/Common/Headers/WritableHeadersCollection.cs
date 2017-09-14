using System;
using System.Collections.Generic;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.HttpContent;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Headers
{
	/// <summary>
	/// Коллекция заголовков, которую заполняет пользователь и использует библиотека.
	/// </summary>
	public abstract class WritableHeadersCollection
	{
        /// <summary>
        /// <para>Устанавливает значение заголовка, перезаписывая текущее в случае его наличия.</para>
        /// <para>С помощью этого метода нельзя добавлять заголовки, доступные через свойства, а также заголовки типа Content-XXX (см. <see cref="IHttpContent"/>)</para>
        /// </summary>
        public void SetCustomHeader(string headerName, string headerValue)
		{
			Preconditions.EnsureNotNull(headerName, "headerName");
			Preconditions.EnsureNotNull(headerValue, "headerValue");

            if (GetRestrictedHeaderNames().Contains(headerName))
                throw new ArgumentException("headerName", $"Attempted to add custom header with restricted name '{headerName}'. It must be set via a property.");

			ObtainCustomHeaders();

		    var index = FindCustomHeaderIndex(headerName);
		    if (index >= 0)
		    {
                CustomHeaders[index] = new KeyValuePair<string, string>(headerName, headerValue);
            }
            else
		    {
		        CustomHeaders.Add(new KeyValuePair<string, string>(headerName, headerValue));
		    }
		}

	    protected void ObtainCustomHeaders()
	    {
	        if (CustomHeaders == null)
	            CustomHeaders = new List<KeyValuePair<string, string>>(2);
	    }

	    public string this[string key]
	    {
	        get
	        {
	            if (CustomHeaders == null)
	                return null;

	            var index = FindCustomHeaderIndex(key);
	            if (index < 0)
	                return null;

	            return CustomHeaders[index].Value;
	        }
	    }

	    protected abstract HashSet<string> GetRestrictedHeaderNames(); 

		internal List<KeyValuePair<string, string>> CustomHeaders { get; private set; }

	    private int FindCustomHeaderIndex(string name)
	    {
            for (var i = 0; i < CustomHeaders.Count; i++)
            {
                var pair = CustomHeaders[i];

                if (string.Equals(pair.Key, name, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

	        return -1;
	    }
	}
}