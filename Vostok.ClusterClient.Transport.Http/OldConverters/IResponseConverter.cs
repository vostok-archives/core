using Vostok.Clusterclient.Model;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Response;

namespace Vostok.ClusterClient.Transport.Http.OldConverters
{
    internal interface IResponseConverter
    {
        Response Convert(HttpResponse response);
    }
}