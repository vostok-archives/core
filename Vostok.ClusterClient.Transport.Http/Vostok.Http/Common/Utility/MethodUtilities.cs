namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility
{
	internal static class MethodUtilities
	{
		public static bool MethodAllowsBody(HttpMethod method)
		{
			switch(method)
			{
				case HttpMethod.POST:
				case HttpMethod.PUT:
				case HttpMethod.PATCH:
				case HttpMethod.DELETE:
					return true;
				default:
					return false;
			}
		}
	
	    public static string GetString(HttpMethod method)
	    {
	        switch (method)
	        {
	            case HttpMethod.HEAD: return "HEAD";
	            case HttpMethod.GET: return "GET";
                case HttpMethod.POST: return "POST";
                case HttpMethod.PUT: return "PUT";
                case HttpMethod.PATCH: return "PATCH";
                case HttpMethod.DELETE: return "DELETE";
                case HttpMethod.TRACE: return "TRACE";
                case HttpMethod.OPTIONS: return "OPTIONS";
                case HttpMethod.STOP: return "STOP";
                default: return method.ToString();
	        }
	    }
	}
}