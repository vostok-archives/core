using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Kontur.Clusterclient.Core.Model;
using Kontur.Net.Http;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Xunit;

namespace Kontur.Clusterclient.Transport.Http
{
    [TestFixture]
    internal class VostokHttpTransport_Tests
    {
        private Request request;
        private Response response;

        private HttpRequest implRequest;
        private HttpResponse implResponse;

        private IRequestConverter requestConverter;
        private IResponseConverter responseConverter;
        private IHttpClient httpClient;
        private VostokHttpTransport transport;

        public VostokHttpTransport_Tests()
        {
            request = Request.Get("http://replica/foo/bar");
            response = new Response(ResponseCode.Ok);

            implRequest = new HttpRequest(HttpMethod.GET, "http://replica/foo/bar");
            implResponse = new HttpResponse(HttpResponseCode.Ok);

            requestConverter = Substitute.For<IRequestConverter>();
            requestConverter.Convert(request).Returns(implRequest);

            responseConverter = Substitute.For<IResponseConverter>();
            responseConverter.Convert(implResponse).Returns(response);

            httpClient = Substitute.For<IHttpClient>();
            httpClient.SendAsync(implRequest, Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(implResponse));

            transport = new VostokHttpTransport(requestConverter, responseConverter, httpClient);
        }

        [Fact]
        public void Should_convert_request()
        {
            Send();

            requestConverter.Received(1).Convert(request);
        }

        [Fact]
        public void Should_send_converted_request_with_correct_timeout_using_http_client()
        {
            Send();

            httpClient.Received(1).SendAsync(implRequest, 5.Seconds(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Should_convert_response()
        {
            Send();

            responseConverter.Received(1).Convert(implResponse);
        }

        [Fact]
        public void Should_return_converted_response()
        {
            Send().Should().BeSameAs(response);
        }

        private Response Send()
        {
            return transport.SendAsync(request, 5.Seconds(), CancellationToken.None).GetAwaiter().GetResult();
        }
    }
}
