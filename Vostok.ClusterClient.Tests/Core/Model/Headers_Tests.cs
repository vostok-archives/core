using System;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Model;

namespace Vostok.ClusterClient.Tests.Core.Model
{
    public class Headers_Tests
    {
        [Test]
        public void Empty_instance_should_not_contain_headers()
        {
            Headers.Empty.Should().BeEmpty();
        }

        [Test]
        public void Empty_instance_should_have_zero_count()
        {
            Headers.Empty.Count.Should().Be(0);
        }

        [Test]
        public void Should_be_able_to_add_a_header_to_empty_instance()
        {
            var headers = Headers.Empty.Set("k", "v");

            headers.Count.Should().Be(1);
            headers["k"].Should().Be("v");
        }

        [Test]
        public void Should_not_modify_empty_instance_when_deriving_from_it()
        {
            Headers.Empty.Set("k", "v");

            Headers.Empty.Should().BeEmpty();

            Headers.Empty.Count.Should().Be(0);
        }

        [Test]
        public void Should_be_case_sensitive_when_comparing_header_names()
        {
            var headers = new Headers(1)
                .Set("key", "value")
                .Set("KEY", "value");

            headers.Should().HaveCount(2);

            headers.Names.Should().Equal("key", "KEY");
        }

        [Test]
        public void Set_should_be_able_to_expand_beyond_initial_capacity()
        {
            var headers = new Headers(1)
                .Set("k1", "v1")
                .Set("k2", "v2")
                .Set("k3", "v3")
                .Set("k4", "v4")
                .Set("k5", "v5");

            headers.Count.Should().Be(5);

            headers.Names.Should().Equal("k1", "k2", "k3", "k4", "k5");
        }

        [Test]
        public void Set_should_return_same_instance_when_replacing_a_header_with_same_value()
        {
            var headersBefore = Headers.Empty
                .Set("k1", "v1")
                .Set("k2", "v2")
                .Set("k3", "v3");

            var headersAfter = headersBefore.Set("k2", "v2");

            headersAfter.Should().BeSameAs(headersBefore);
        }

        [Test]
        public void Set_should_replace_existing_header_when_provided_with_a_different_value()
        {
            var headersBefore = Headers.Empty
                .Set("k1", "v1")
                .Set("k2", "v2")
                .Set("k3", "v3");

            var headersAfter = headersBefore.Set("k2", "vx");

            headersAfter.Should().NotBeSameAs(headersBefore);
            headersAfter.Count.Should().Be(3);
            headersAfter["k1"].Should().Be("v1");
            headersAfter["k2"].Should().Be("vx");
            headersAfter["k3"].Should().Be("v3");
        }

        [Test]
        public void Set_should_not_modify_base_instance_when_deriving_from_it_by_replacing_existing_header()
        {
            var headersBefore = Headers.Empty
                .Set("k1", "v1")
                .Set("k2", "v2")
                .Set("k3", "v3");

            var headersAfter = headersBefore.Set("k2", "vx");

            headersAfter.Should().NotBeSameAs(headersBefore);
            headersBefore.Count.Should().Be(3);
            headersBefore["k1"].Should().Be("v1");
            headersBefore["k2"].Should().Be("v2");
            headersBefore["k3"].Should().Be("v3");
        }

        [Test]
        public void Set_should_be_able_to_grow_headers_when_adding_unique_names_without_spoiling_base_instance()
        {
            var headers = new Headers(0);

            for (var i = 0; i < 100; i++)
            {
                var newName = "name-" + i;
                var newValue = "value-" + i;

                var newHeaders = headers.Set(newName, newValue);

                newHeaders.Should().NotBeSameAs(headers);
                newHeaders.Count.Should().Be(i + 1);
                newHeaders[newName].Should().Be(newValue);

                headers.Count.Should().Be(i);
                headers[newName].Should().BeNull();
                headers = newHeaders;
            }
        }

        [Test]
        public void Set_should_correctly_handle_forking_multiple_instances_from_a_single_base()
        {
            var headersBefore = Headers.Empty
                .Set("k1", "v1")
                .Set("k2", "v2")
                .Set("k3", "v3");

            var headersAfter1 = headersBefore.Set("k4", "v4-1");
            var headersAfter2 = headersBefore.Set("k4", "v4-2");

            headersBefore.Count.Should().Be(3);
            headersBefore["k4"].Should().BeNull();

            headersAfter1.Should().NotBeSameAs(headersBefore);
            headersAfter1.Count.Should().Be(4);
            headersAfter1["k4"].Should().Be("v4-1");

            headersAfter2.Should().NotBeSameAs(headersBefore);
            headersAfter2.Should().NotBeSameAs(headersAfter1);
            headersAfter2.Count.Should().Be(4);
            headersAfter2["k4"].Should().Be("v4-2");
        }

        [Test]
        public void Remove_should_correctly_remove_first_header()
        {
            var headersBefore = Headers.Empty
                .Set("k1", "v1")
                .Set("k2", "v2")
                .Set("k3", "v3")
                .Set("k4", "v4")
                .Set("k5", "v5");

            var headersAfter = headersBefore.Remove("k1");

            headersAfter.Should().NotBeSameAs(headersBefore);
            headersAfter.Should().HaveCount(4);
            headersAfter["k1"].Should().BeNull();
        }

        [Test]
        public void Remove_should_correctly_remove_last_header()
        {
            var headersBefore = Headers.Empty
                .Set("k1", "v1")
                .Set("k2", "v2")
                .Set("k3", "v3")
                .Set("k4", "v4")
                .Set("k5", "v5");

            var headersAfter = headersBefore.Remove("k5");

            headersAfter.Should().NotBeSameAs(headersBefore);
            headersAfter.Should().HaveCount(4);
            headersAfter["k5"].Should().BeNull();
        }

        [Test]
        public void Remove_should_correctly_remove_a_header_from_the_middle()
        {
            var headersBefore = Headers.Empty
                .Set("k1", "v1")
                .Set("k2", "v2")
                .Set("k3", "v3")
                .Set("k4", "v4")
                .Set("k5", "v5");

            var headersAfter = headersBefore.Remove("k3");

            headersAfter.Should().NotBeSameAs(headersBefore);
            headersAfter.Should().HaveCount(4);
            headersAfter["k3"].Should().BeNull();
        }

        [Test]
        public void Remove_should_correctly_remove_the_only_header_in_collection()
        {
            var headersBefore = Headers.Empty
                .Set("k1", "v1");

            var headersAfter = headersBefore.Remove("k1");

            headersAfter.Should().NotBeSameAs(headersBefore);
            headersAfter.Count.Should().Be(0);
            headersAfter.Should().BeEmpty();
        }

        [Test]
        public void Remove_should_return_same_instance_when_removing_a_header_which_is_not_present()
        {
            var headersBefore = Headers.Empty
                .Set("k1", "v1")
                .Set("k2", "v2")
                .Set("k3", "v3")
                .Set("k4", "v4")
                .Set("k5", "v5");

            var headersAfter = headersBefore.Remove("k6");

            headersAfter.Should().BeSameAs(headersBefore);
        }

        [Test]
        public void Remove_should_not_spoil_base_insance()
        {
            var headersBefore = Headers.Empty
                .Set("k1", "v1")
                .Set("k2", "v2")
                .Set("k3", "v3")
                .Set("k4", "v4")
                .Set("k5", "v5");

            headersBefore.Remove("k1");
            headersBefore.Remove("k2");
            headersBefore.Remove("k3");
            headersBefore.Remove("k4");
            headersBefore.Remove("k5");

            headersBefore.Should().HaveCount(5);
            headersBefore["k1"].Should().Be("v1");
            headersBefore["k2"].Should().Be("v2");
            headersBefore["k3"].Should().Be("v3");
            headersBefore["k4"].Should().Be("v4");
            headersBefore["k5"].Should().Be("v5");
        }

        [Test]
        public void Indexer_should_return_null_when_headers_are_empty()
        {
            Headers.Empty["name"].Should().BeNull();
        }

        [Test]
        public void Indexer_should_return_null_when_header_with_given_name_does_not_exist()
        {
            var headers = Headers.Empty
                .Set("name1", "value1")
                .Set("name2", "value2");

            headers["name3"].Should().BeNull();
        }

        [Test]
        public void Indexer_should_return_correct_values_for_existing_header_names()
        {
            var headers = Headers.Empty
                .Set("name1", "value1")
                .Set("name2", "value2");

            headers["name1"].Should().Be("value1");
            headers["name2"].Should().Be("value2");
        }

        [Test]
        public void Indexer_should_be_case_sensitive_when_comparing_header_names()
        {
            var headers = Headers.Empty
                .Set("name", "value1")
                .Set("NAME", "value2");

            headers["name"].Should().Be("value1");
            headers["NAME"].Should().Be("value2");
        }

        [Test]
        public void ToString_should_return_empty_string_for_empty_headers()
        {
            Headers.Empty.ToString().Should().BeEmpty();
        }

        [Test]
        public void ToString_should_return_correct_representation_for_non_empty_headers()
        {
            var headers = Headers.Empty
                .Set("name1", "value1")
                .Set("name2", "value2");

            headers.ToString().Should().Be("name1: value1" + Environment.NewLine + "name2: value2");
        }

        
        [TestCase(HeaderNames.Accept, nameof(HeaderNames.Accept))]
        [TestCase(HeaderNames.Age, nameof(HeaderNames.Age))]
        [TestCase(HeaderNames.Authorization, nameof(HeaderNames.Authorization))]
        [TestCase(HeaderNames.ContentEncoding, nameof(HeaderNames.ContentEncoding))]
        [TestCase(HeaderNames.ContentLength, nameof(HeaderNames.ContentLength))]
        [TestCase(HeaderNames.ContentType, nameof(HeaderNames.ContentType))]
        [TestCase(HeaderNames.ContentRange, nameof(HeaderNames.ContentRange))]
        [TestCase(HeaderNames.Date, nameof(HeaderNames.Date))]
        [TestCase(HeaderNames.ETag, nameof(HeaderNames.ETag))]
        [TestCase(HeaderNames.Host, nameof(HeaderNames.Host))]
        [TestCase(HeaderNames.LastModified, nameof(HeaderNames.LastModified))]
        [TestCase(HeaderNames.Location, nameof(HeaderNames.Location))]
        [TestCase(HeaderNames.Range, nameof(HeaderNames.Range))]
        [TestCase(HeaderNames.Referer, nameof(HeaderNames.Referer))]
        [TestCase(HeaderNames.RetryAfter, nameof(HeaderNames.RetryAfter))]
        [TestCase(HeaderNames.Server, nameof(HeaderNames.Server))]
        [TestCase(HeaderNames.TransferEncoding, nameof(HeaderNames.TransferEncoding))]
        [TestCase(HeaderNames.UserAgent, nameof(HeaderNames.UserAgent))]
        [TestCase(HeaderNames.WWWAuthenticate, nameof(HeaderNames.WWWAuthenticate))]
        public void Specific_header_getter_should_return_correct_value(string headerName, string propertyName)
        {
            var headersWith = Headers.Empty.Set(headerName, "value");
            var headersWithout = Headers.Empty.Set(Guid.NewGuid().ToString(), "value");

            var property = typeof (Headers).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

            property.Should().NotBeNull();
            // ReSharper disable once PossibleNullReferenceException
            property.GetValue(headersWith).Should().Be("value");
            property.GetValue(headersWithout).Should().BeNull();
        }
    }
}
