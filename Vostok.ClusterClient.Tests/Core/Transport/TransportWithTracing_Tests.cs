using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Transport;
using Vostok.Tracing;

namespace Vostok.ClusterClient.Tests.Core.Transport
{
    public class TransportWithTracing_Tests
    {
        private ITraceReporter traceReporter;
        private ITransport transport;
        private TransportWithTracing transportWithTracing;

        [SetUp]
        public void SetUp()
        {
            traceReporter = Substitute.For<ITraceReporter>();
            transport = Substitute.For<ITransport>();
            transportWithTracing = new TransportWithTracing(transport);
            Trace.Configuration.Reporter = traceReporter;
        }

        [Test]
        public async Task SendAsync_should_create_trace()
        {
            var request = new Request("POST", new Uri("http://vostok/process?p=p1"),new Content(new byte[10]));
            var timeout = TimeSpan.FromHours(1);
            var cancellationToken = new CancellationToken();
            var response = new Response(ResponseCode.BadRequest);
            transport.SendAsync(request, timeout, cancellationToken).Returns(response);
            var expectedAnnotations = new Dictionary<string, string>
            {
                [TracingAnnotationNames.Kind] = "http-client",
                [TracingAnnotationNames.Component] = "cluster-client",
                [TracingAnnotationNames.HttpUrl] = "http://vostok/process",
                [TracingAnnotationNames.HttpMethod] = "POST",
                [TracingAnnotationNames.HttpRequestContentLength] = "10",
                [TracingAnnotationNames.HttpResponseContentLength] = "0",
                [TracingAnnotationNames.HttpCode] = "400"
            };
            traceReporter.SendSpan(Arg.Do<Span>(span =>
            {
                span.Annotations.ShouldBeEquivalentTo(expectedAnnotations);
            }));

            var actual = await transportWithTracing.SendAsync(request, timeout, cancellationToken).ConfigureAwait(false);

            actual.Should().Be(response);
            traceReporter.Received().SendSpan(Arg.Any<Span>());
        }
    }
}