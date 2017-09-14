using System.Text;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility
{
	internal static class EncodingFactory
	{
		public static Encoding GetDefault()
		{
			return Encoding.UTF8;
		}
	}
}