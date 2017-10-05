using System;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Airlock
{
    public class RoutingKey_Tests
    {
        [TestCase("Diadoc№1", "прод","service2", "diadoc-1.----.service2")]
        [TestCase("\x001", "a", "srv", "-.a.srv")]
        [TestCase("aa\x999", "Ш", "#srv","aa-.-.-srv")]
        public void CreatePrefix_returns_prefix(string project, string environment, string service, string prefix)
        {
            Assert.AreEqual(prefix, RoutingKey.CreatePrefix(project, environment, service));
        }

        [TestCase(null, "", "#srv")]
        [TestCase("", null, "#srv")]
        [TestCase("", "s", "#srv")]
        public void CreatePrefix_fails_if_null_item(string project, string environment, string service)
        {
            Assert.Throws<Exception>(() => RoutingKey.CreatePrefix(project, environment, service));
        }

        [TestCase("diadoc-1", "s", "service2", "diadoc-1.s.service2", new string[]{})]
        //[TestCase("", "", "srv", "..srv")]
        public void Parse_returns_parts(string project, string environment, string service, string routingKey, string[] suffix)
        {
            Assert.True(RoutingKey.TryParse(routingKey, out var project2, out var environment2, out var service2, out var suffix2), "TryParse");
            Assert.AreEqual(project, project2, "project");
            Assert.AreEqual(environment, environment2, "environment");
            Assert.AreEqual(service, service2, "service");
            suffix2.ShouldBeEquivalentTo(suffix);
        }
    }
}