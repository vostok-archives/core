using System.Net.Http;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Transport.Http
{
    internal static class SystemNetHttpResponseConverter
    {
        public static async Task<Response> ConvertAsync(HttpResponseMessage message)
        {
            var convertedCode = (ResponseCode)message.StatusCode;
            var headers = Headers.Empty;
            var content = Content.Empty;

            if (message.Content != null)
            {
                var bytes = await message.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                content = new Content(bytes);
                headers = headers.Append(message.Content.Headers);
            }

            headers = headers.Append(message.Headers);
            return new Response(convertedCode, content, headers);
        }
    }
}