using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Vostok.Flow.DistributedContextSerializer;
using Xunit;

namespace Vostok.Flow
{
    public class DistributedContext_Test
    {
        public static IEnumerable<object[]> GenerateData()
        {
            yield return new object[] {new Guid("9B47124B-8015-4E2D-B6C9-0168F10D45FB"), "guid|9b47124b-8015-4e2d-b6c9-0168f10d45fb" };
            yield return new object[] {"str", "string|str"};
            yield return new object[] {(byte) 1, "byte|1"};
            yield return new object[] {1, "int|1"};
            yield return new object[] {143L, "long|143"};
            yield return new object[] {'a', "char|a"};
            yield return new object[] {1.1, "double|1,1"};
            yield return new object[] {1.2f, "float|1,2"};
            yield return new object[] {new byte[]{ 13, 15, 17, 44, 35}, "BytesArray|DQ8RLCM=" };
        }

        [Theory]
        [MemberData(nameof(GenerateData))]
        public void BuildDistributedContext(object obj, string stringValue)
        {
            const string key = "key";
            Context.Properties.SetProperty(key, obj);
            Context.Configuration.DistributedProperties.Add(key);

            var distributedContext = Context.BuildDistributedContext().ToArray();

            distributedContext.Length.Should().Be(1);
            distributedContext[0].Key.Should().Be(key);
            distributedContext[0].Value.Should().Be(stringValue);
        }

        [Theory]
        [MemberData(nameof(GenerateData))]
        public void SetDistributedContext(object obj, string stringValue)
        {
            const string key = "key";
            var distributedContext = new[]
            {
                new KeyValuePair<string, string>(key, stringValue)
            };
            Context.Configuration.DistributedProperties.Add(key);

            Context.SetDistributedContext(distributedContext);

            var actual = Context.Properties.Current[key];
            actual.ShouldBeEquivalentTo(obj);
        }

        [Fact]
        public void CheckCompletenessTestingTypes()
        {
            var defaultTypeвSerializers = GetAllDefaultTypedSerializers()
                .Select(x => x.Type)
                .ToArray();

            var testingTypes = GenerateData()
                .Select(x => x[0])
                .Select(x => x.GetType())
                .Distinct()
                .ToArray();

            testingTypes.Should().Contain(defaultTypeвSerializers);
        }

        [Fact]
        public void CheckDuplicateSerializersByType()
        {
            var duplicateSerializers = GetAllDefaultTypedSerializers()
                .GroupBy(x => x.Type)
                .Where(x => x.Count() > 1)
                .SelectMany(x => x)
                .ToArray();

            duplicateSerializers.Should().BeEmpty();
        }

        [Fact]
        public void CheckDuplicateSerializersById()
        {
            var duplicateSerializers = GetAllDefaultTypedSerializers()
                .GroupBy(x => x.Id)
                .Where(x => x.Count() > 1)
                .SelectMany(x => x)
                .ToArray();

            duplicateSerializers.Should().BeEmpty();
        }

        private static ITypedSerializer[] GetAllDefaultTypedSerializers()
        {
            return Assembly.GetAssembly(typeof(ITypedSerializer))
                .GetTypes()
                .Where(type => !type.IsAbstract && type.IsClass)
                .Where(type => typeof(ITypedSerializer).IsAssignableFrom(type))
                .Select(Activator.CreateInstance)
                .OfType<ITypedSerializer>()
                .ToArray();
        }
    }
}