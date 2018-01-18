using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient;
using Vostok.Clusterclient.Model;

namespace Vostok.ClusterClient.Tests.Core.Model
{
    public class Response_Tests
    {
        [Test]
        public void Headers_property_should_return_empty_headers_instead_of_null()
        {
            new Response(ResponseCode.Ok).Headers.Should().BeSameAs(Headers.Empty);
        }

        [Test]
        public void Content_property_should_return_empty_content_instead_of_null()
        {
            new Response(ResponseCode.Ok).Content.Should().BeSameAs(Content.Empty);
        }

        
        [TestCase(ResponseCode.Ok)]
        [TestCase(ResponseCode.Created)]
        [TestCase(ResponseCode.Accepted)]
        [TestCase(ResponseCode.NoContent)]
        [TestCase(ResponseCode.ResetContent)]
        [TestCase(ResponseCode.PartialContent)]
        [TestCase(ResponseCode.NonAuthoritativeInformation)]
        public void IsSuccessful_should_return_true_for_codes_from_2xx_family(ResponseCode code)
        {
            new Response(code).IsSuccessful.Should().BeTrue();
        }

        
        [TestCase(ResponseCode.Continue)]
        [TestCase(ResponseCode.MovedPermanently)]
        [TestCase(ResponseCode.NotFound)]
        [TestCase(ResponseCode.InternalServerError)]
        public void IsSuccessful_should_return_false_for_codes_from_families_other_than_2xx(ResponseCode code)
        {
            new Response(code).IsSuccessful.Should().BeFalse();
        }

        [Test]
        public void ToString_should_return_correct_representation_when_omitting_headers()
        {
            var response = new Response(ResponseCode.Ok, headers: Headers.Empty.Set("name", "value"));

            response.ToString(false).Should().Be("200 Ok");
        }

        [Test]
        public void ToString_should_return_correct_representation_when_printing_headers()
        {
            var response = new Response(ResponseCode.Ok, headers: Headers.Empty.Set("name", "value"));

            response.ToString(true).Should().Be("200 Ok" + Environment.NewLine + "name: value");
        }

        [Test]
        public void ToString_should_omit_headers_by_default()
        {
            var response = new Response(ResponseCode.Ok, headers: Headers.Empty.Set("name", "value"));

            response.ToString().Should().Be("200 Ok");
        }

        [Test]
        public void ToString_should_ignore_empty_headers()
        {
            var response = new Response(ResponseCode.Ok, headers: Headers.Empty);

            response.ToString(true).Should().Be("200 Ok");
        }

        [Test]
        public void ToString_should_ignore_null_headers()
        {
            var response = new Response(ResponseCode.Ok);

            response.ToString(true).Should().Be("200 Ok");
        }

        
        [TestCase(ResponseCode.InternalServerError)]
        [TestCase(ResponseCode.BadRequest)]
        [TestCase(ResponseCode.MovedPermanently)]
        [TestCase(ResponseCode.UnknownFailure)]
        public void EnsureSuccessStatusCode_should_throw_an_exception_for_non_2xx_code(ResponseCode code)
        {
            Action action = () => new Response(code).EnsureSuccessStatusCode();

            action.ShouldThrow<ClusterClientException>();
        }

        
        [TestCase(ResponseCode.Ok)]
        [TestCase(ResponseCode.Created)]
        [TestCase(ResponseCode.NoContent)]
        public void EnsureSuccessStatusCode_should_have_no_effect_for_2xx_code(ResponseCode code)
        {
            var response = new Response(code);

            response.EnsureSuccessStatusCode().Should().BeSameAs(response);
        }
    }
}
