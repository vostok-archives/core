﻿using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Commons;

namespace Vostok.Common
{
    public class UrlExtensions_Tests
    {
        [TestCase("http://vostok/process?p=p1", "http://vostok/process")]
        [TestCase("http://vostok/guids/EABC6944-2D64-4590-873F-42EE84651713/process/", "http://vostok/guids/{guid}/process/")]
        [TestCase("http://vostok/guids/eabc6944-2d64-4590-873f-42ee84651713/process/", "http://vostok/guids/{guid}/process/")]
        [TestCase("http://vostok/page/1-100/process/", "http://vostok/page/{num}/process/")]
        [TestCase("http://vostok/message/hello+world/process/", "http://vostok/message/{enc}/process/")]
        [TestCase("http://vostok/message/%D0%BF%D1%80%D0%B8%D0%B2%D0%B5%D1%82%2F/process/", "http://vostok/message/{enc}/process/")]
        [TestCase("http://vostok/binary/1234567890ABCDEF/process/", "http://vostok/binary/{hex}/process/")]
        [TestCase("http://vostok/binary/1234567890abcdef/process/", "http://vostok/binary/{hex}/process/")]
        public void Normalize_should_return_normalizedUrl(string urlString, string normalizeUrl)
        {
            var uri = new Uri(urlString);

            var actual = uri.Normalize();

            actual.Should().Be(normalizeUrl);
        }
    }
}