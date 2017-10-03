using System;
using System.Threading;
using FluentAssertions;
using NSubstitute;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Transport;
using Xunit;

namespace Vostok.Clusterclient.Core.Transport
{
    public class TransportWithAuxiliaryHeaders_Tests
    {
        private readonly IClusterClientConfiguration configuration;
        private readonly ITransport underlyingTransport;
        private readonly TransportWithAuxiliaryHeaders transport;
        private readonly Request request;

        public TransportWithAuxiliaryHeaders_Tests()
        {
            configuration = Substitute.For<IClusterClientConfiguration>();
            underlyingTransport = Substitute.For<ITransport>();
            transport = new TransportWithAuxiliaryHeaders(underlyingTransport, configuration);

            request = Request.Get("http://foo/bar");
        }

        [Fact]
        public void Should_forward_original_request_when_all_auxiliary_headers_are_disabled()
        {
            Send();

            CheckRequest(request);
        }

        [Fact]
        public void Should_include_request_timeout_header_when_asked_to()
        {
            configuration.IncludeRequestTimeoutHeader.Returns(true);

            Send();

            CheckRequest(r => r?.Headers?[HeaderNames.XKonturRequestTimeout] == 1.Minutes().Ticks.ToString());
        }

        [Fact]
        public void Should_include_client_identity_header_when_asked_to()
        {
            configuration.IncludeClientIdentityHeader.Returns(true);

            Send();

            CheckRequest(r => r?.Headers?[HeaderNames.XKonturClientIdentity] != null);
        }

        private void Send()
        {
            transport.SendAsync(request, 1.Minutes(), CancellationToken.None).GetAwaiter().GetResult();
        }

        private void CheckRequest(Request expected)
        {
            underlyingTransport.Received().SendAsync(expected, 1.Minutes(), CancellationToken.None);
        }

        private void CheckRequest(Predicate<Request> expected)
        {
            underlyingTransport.Received().SendAsync(Arg.Is<Request>(r => expected(r)), 1.Minutes(), CancellationToken.None);
        }
    } 
}