using System;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Airlock.Client.Tests
{
    // todo (avk 06.10.2017) add tests for all methods for  case insensetivity
    public class RoutingKey_Tests
    {
        [TestCase("Diadoc№1", "прод","service2", "diadoc-1.----.service2")]
        [TestCase("!", "a", "srv", "-.a.srv")]
        [TestCase("aa\x999", "Ш", "#srv","aa-.-.-srv")]
        public void CreatePrefix_returns_prefix(string project, string environment, string service, string prefix)
        {
            Assert.AreEqual(prefix, RoutingKey.CreatePrefix(project, environment, service));
            Assert.AreEqual(prefix, RoutingKey.Create(project, environment, service));
        }

        [TestCase(null, "", "#srv")]
        [TestCase("", null, "#srv")]
        [TestCase("", "s", "#srv")]
        public void CreatePrefix_fails_if_empty_item(string project, string environment, string service)
        {
            Assert.Throws<ArgumentException>(() => RoutingKey.CreatePrefix(project, environment, service));
        }

        [TestCase("diadoc-1", "s", "service2", new string[]{}, "diadoc-1.s.service2")]
        [TestCase("diadoc-1", "s", "service2", new string[]{}, "diadoc-1.s.service2")]
        [TestCase("diadoc", "prod", "service", new []{ "foo", "bar" }, "diadoc.prod.service.foo.bar")]
        [TestCase("diadoc", "prod", "service", new []{ "foo-bar"}, "diadoc.prod.service.foo-bar")]
        public void Parse_returns_parts(string project, string environment, string service, string[] suffix, string routingKey)
        {
            Assert.True(RoutingKey.TryParse(routingKey, out var project2, out var environment2, out var service2, out var suffix2), "TryParse");
            Assert.AreEqual(project, project2, "project");
            Assert.AreEqual(environment, environment2, "environment");
            Assert.AreEqual(service, service2, "service");
            suffix2.ShouldBeEquivalentTo(suffix);
        }

        [TestCase("asdasda..ssa")]
        [TestCase("asdasda..ssa.asdas.asda.")]
        [TestCase("asdasda.")]
        [TestCase("привет.медвед.хелло!")]
        [TestCase(".asdasda.")]
        [TestCase("as.das")]
        [TestCase("as")]
        [TestCase(".")]
        [TestCase("")]
        public void Parse_fails_if_invalid_key(string routingKey)
        {
            Assert.False(RoutingKey.TryParse(routingKey, out var _, out var _, out var _, out var _));
            Assert.Throws<InvalidOperationException>(() => RoutingKey.Parse(routingKey, out var _, out var _, out var _, out var _));
        }

        [TestCase("diadoc.prod.serv.suff", new []{"newsuff"}, "diadoc.prod.serv.newsuff")]
        [TestCase("diadoc.prod.serv.suff", new string[]{}, "diadoc.prod.serv")]
        [TestCase("diadoc.prod.serv.suff.jasklajsd.asdasd", new []{"my","suff"}, "diadoc.prod.serv.my.suff")]
        public void ReplaceSuffixTest(string key, string[] newSuffix, string newKey)
        {
            Assert.AreEqual(newKey, RoutingKey.ReplaceSuffix(key, newSuffix)); 
        }

        [TestCase("project.env.service.last-part", "last-part")]
        [TestCase("project.env.service.last-part", "Last-Part")]
        [TestCase("project.env.service.Last-part", "last-part")]
        [TestCase("project.env.service.Last-part", "last-part")]
        [TestCase("project.env.service.whatever.last-part", "last-part")]
        public void LastSuffixMatches_True(string routingKey, string lastSuffix)
        {
            RoutingKey.LastSuffixMatches(routingKey, lastSuffix).Should().BeTrue();
        }

        [TestCase("project.env.last-part", "last-part")]
        [TestCase("project.env.service.whatever", "last-part")]
        [TestCase("project.env.service.lastpart", "last-part")]
        [TestCase("project.env.service.last-part.", "last-part")]
        [TestCase("project.env.service.last-part.whatever", "last-part")]
        [TestCase("pro_ject.env.service.last-part", "last-part")]
        public void LastSuffixMatches_False(string routingKey, string lastSuffix)
        {
            RoutingKey.LastSuffixMatches(routingKey, lastSuffix).Should().BeFalse();
        }
    }
}