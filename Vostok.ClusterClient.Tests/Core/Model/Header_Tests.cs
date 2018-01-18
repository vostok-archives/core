using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Model;

namespace Vostok.ClusterClient.Tests.Core.Model
{
    public class Header_Tests
    {
        [Test]
        public void Should_throw_an_error_when_supplied_with_null_name()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Action action = () => new Header(null, "value");

            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void Should_throw_an_error_when_supplied_with_null_value()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Action action = () => new Header("name", null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void ToString_should_return_correct_representation()
        {
            var header = new Header("X-Kontur-External-Url", "http://foo.kontur.ru/bar");

            header.ToString().Should().Be("X-Kontur-External-Url: http://foo.kontur.ru/bar");
        }

        [Test]
        public void Should_have_case_sensitive_equality()
        {
            var header1 = new Header("name", "value");
            var header2 = new Header("name", "value");
            var header3 = new Header("Name", "value");
            var header4 = new Header("name", "Value");

            header1.Should().Be(header2);
            header1.Should().NotBe(header3);
            header1.Should().NotBe(header4);
        }
    }
}
