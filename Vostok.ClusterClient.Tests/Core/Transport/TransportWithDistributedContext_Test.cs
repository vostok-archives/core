using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Transport;
using Vostok.Flow;

namespace Vostok.ClusterClient.Tests.Core.Transport
{
    public class TransportWithDistributedContext_Test
    {
        private const string xDistributedContextSeparator = "-";
        private ITransport transport;
        private TransportWithDistributedContext transportWithDistributedContext;
        private string existedkey;

        [SetUp]
        public void SetUp()
        {
            Context.Configuration.DistributedProperties.Clear();
            transport = Substitute.For<ITransport>();
            transportWithDistributedContext = new TransportWithDistributedContext(transport);
        }

        
        [TestCase("key", "value", HeaderNames.XDistributedContextPrefix + xDistributedContextSeparator + "key", "string|value")]
        [TestCase("ключ", "значение", HeaderNames.XDistributedContextPrefix + xDistributedContextSeparator + "%d0%ba%d0%bb%d1%8e%d1%87", "string|%d0%b7%d0%bd%d0%b0%d1%87%d0%b5%d0%bd%d0%b8%d0%b5")]
        public void SendAsync_should_create_headers_from_DistributedContext_when_headers_is_null(string key, string value, string expectedKey, string expectedValue)
        {
            Context.Configuration.DistributedProperties.Add(key);
            Context.Properties.SetProperty(key, value);

            var request = new Request("GET", new Uri("http://localhost"));
            Header[] actual = null;
            transport.SendAsync(Arg.Do<Request>(x => { actual = x.Headers?.ToArray(); }), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()).Returns(new Task<Response>(() => null));

            transportWithDistributedContext.SendAsync(request, TimeSpan.Zero, CancellationToken.None);

            actual.Length.Should().Be(1);
            actual[0].Name.Should().Be(expectedKey);
            actual[0].Value.Should().Be(expectedValue);
        }

        [Test]
        public void SendAsync_should_not_create_headers_when_DistributedContext_is_empty_when_headers_is_null()
        {
            var request = new Request("GET", new Uri("http://localhost"));
            Headers actual = null;
            transport.SendAsync(Arg.Do<Request>(x => { actual = x.Headers; }), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()).Returns(new Task<Response>(() => null));

            transportWithDistributedContext.SendAsync(request, TimeSpan.Zero, CancellationToken.None);

            actual.Should().BeNull();
        }

        [Test]
        public void SendAsync_should_add_headers_from_DistributedContext_when_headers_is_not_empty()
        {
            const string key1 = "key1";
            const string key2 = "key2";
            Context.Configuration.DistributedProperties.Add(key1);
            Context.Configuration.DistributedProperties.Add(key2);
            Context.Properties.SetProperty(key1, "value1");
            Context.Properties.SetProperty(key2, "value2");

            existedkey = "existedKey";
            var request = new Request("GET", new Uri("http://localhost")).WithHeader(existedkey, "existedValue");
            Headers actual = null;
            transport.SendAsync(Arg.Do<Request>(x => { actual = x.Headers; }), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()).Returns(new Task<Response>(() => null));
            var expectedKeys = new[] {existedkey, HeaderNames.XDistributedContextPrefix + xDistributedContextSeparator + key1, HeaderNames.XDistributedContextPrefix + xDistributedContextSeparator + key2 };

            transportWithDistributedContext.SendAsync(request, TimeSpan.Zero, CancellationToken.None);

            actual.Names.Should().BeEquivalentTo(expectedKeys);
        }

        [Test]
        public void SendAsync_should_not_add_headers_from_DistributedContext_when_key_exists()
        {
            const string key = "key";
            const string headerKey = HeaderNames.XDistributedContextPrefix + xDistributedContextSeparator + key;

            const string oldvalue = "oldValue";

            Context.Configuration.DistributedProperties.Add(key);
            Context.Properties.SetProperty(key, "newValue");

            var request = new Request("GET", new Uri("http://localhost")).WithHeader(headerKey, oldvalue);
            Headers actual = null;
            transport.SendAsync(Arg.Do<Request>(x => { actual = x.Headers; }), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()).Returns(new Task<Response>(() => null));

            transportWithDistributedContext.SendAsync(request, TimeSpan.Zero, CancellationToken.None);

            actual[headerKey].Should().Be(oldvalue);
        }
    }
}