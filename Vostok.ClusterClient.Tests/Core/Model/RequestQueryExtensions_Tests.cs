using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Model;

namespace Vostok.ClusterClient.Tests.Core.Model
{
    public class RequestQueryExtensions_Tests
    {
        private Request request;

        [SetUp]
        public void SetUp()
        {
            request = Request.Get("foo/bar?a=1");
        }

        [Test]
        public void WithAdditionalQueryParameter_should_correctly_add_parameter_with_string_value()
        {
            request.WithAdditionalQueryParameter("b", "2").Url.ToString().Should().Be("foo/bar?a=1&b=2");
        }

        [Test]
        public void WithAdditionalQueryParameter_should_correctly_add_parameter_with_typed_object_value()
        {
            request.WithAdditionalQueryParameter("b", 2).Url.ToString().Should().Be("foo/bar?a=1&b=2");
        }
    }
}
