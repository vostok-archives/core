namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.HttpContent
{
    internal static class HttpContentExtentions
    {
        public static string GetContentTypeHeaderValue(this IHttpContent content)
        {
            var contentType = (content.ContentType ?? ContentType.OctetStream).ToString();
            if (content.Charset != null)
            {
                contentType += "; charset=" + content.Charset.WebName;
            }
            return contentType;
        }
    }
}