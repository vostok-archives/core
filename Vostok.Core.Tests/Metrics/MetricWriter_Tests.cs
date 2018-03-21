using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vstk.Commons.Collections;
using Vstk.Flow;

namespace Vstk.Metrics
{
    public class MetricWriter_Tests
    {
        private Action<MetricEvent> commitAction;
        private MetricEventWriter writer;
        private IPool<MetricEventWriter> originPool;
        private IMetricConfiguration metricConfiguration;

        [SetUp]
        public void SetUp()
        {
            originPool = Substitute.For<IPool<MetricEventWriter>>();
            metricConfiguration = Substitute.For<IMetricConfiguration>();
            writer = new MetricEventWriter(
                originPool,
                metricConfiguration,
                m => commitAction?.Invoke(m));
        }

        [Test]
        public void Initialize_should_set_timestamp()
        {
            var timestamp = new DateTimeOffset(2017, 10, 04, 00, 00, 00, TimeSpan.Zero);
            writer.SetTimestamp(timestamp);

            writer.Initialize();

            commitAction = me =>
            {
                me.Timestamp.Should().NotBe(timestamp);
            };
            writer.Commit();
        }

        [Test]
        public void Initialize_should_set_host_tag()
        {
            writer.Initialize();

            commitAction = me =>
            {
                me.Tags.ContainsKey("host").Should().BeTrue();
            };
            writer.Commit();
        }

        [Test]
        public void Initialize_should_enrich_tags_from_context_using_whitelist()
        {
            metricConfiguration.ContextFieldsWhitelist.Returns(new HashSet<string>
            {
                "my_tag"
            });
            commitAction = me =>
            {
                me.Tags.ContainsKey("my_tag").Should().BeTrue();
            };

            using (Context.Properties.Use("my_tag", "value"))
            {
                writer.Initialize();
                writer.Commit();
            }
        }

        [Test]
        public void Can_set_value_set_tag_and_set_timestamp()
        {
            writer.SetTag("abc", "def");
            writer.SetValue("abc", 42);
            var timestamp = new DateTimeOffset(2017, 10, 04, 00, 00, 00, TimeSpan.Zero);
            writer.SetTimestamp(timestamp);

            commitAction = me =>
            {
                me.Tags["abc"].Should().Be("def");
                me.Values["abc"].Should().Be(42);
                me.Timestamp.Should().Be(timestamp);
            };
            writer.Commit();
        }

        [Test]
        public void After_commit_writer_is_cleared()
        {
            writer.SetTag("abc", "def");
            writer.SetValue("abc", 42);
            writer.Commit();
            writer.SetTag("abc2", "def2");
            writer.SetValue("abc2", 43);

            commitAction = me =>
            {
                me.Tags.ContainsKey("abc").Should().BeFalse();
                me.Tags["abc2"].Should().Be("def2");
                me.Values.ContainsKey("abc").Should().BeFalse();
                me.Values["abc2"].Should().Be(43);
            };
            writer.Commit();
        }

        [Test]
        public void Writer_returns_to_pool_after_commit()
        {
            writer.Commit();
            originPool.Received(1).Release(Arg.Is(writer));
        }
    }
}