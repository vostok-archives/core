using Vostok.Clusterclient.Model;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Requests;

namespace Vostok.ClusterClient.Transport.Http.OldConverters
{
    internal interface IRequestConverter
    {
        HttpRequest Convert(Request request);
    }
}
