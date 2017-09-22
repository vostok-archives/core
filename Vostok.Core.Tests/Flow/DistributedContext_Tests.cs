using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Vostok.Flow.Serializers;
using Xunit;

namespace Vostok.Flow
{
    public class DistributedContext_Tests
    {
        private static IEnumerable<object[]> GenerateData()
        {
            yield return new object[] {new Guid("9B47124B-8015-4E2D-B6C9-0168F10D45FB"), "guid|9b47124b-8015-4e2d-b6c9-0168f10d45fb" };
            yield return new object[] {new Guid?(new Guid("9B47124B-8015-4E2D-B6C9-0168F10D45FB")), "guid|9b47124b-8015-4e2d-b6c9-0168f10d45fb" };
            yield return new object[] {"str", "string|str"};
            yield return new object[] {"", "string|"};
            yield return new object[] {(byte) 1, "ubyte|1"};
            yield return new object[] {1, "int32|1"};
            yield return new object[] {143L, "int64|143"};
            yield return new object[] {'a', "char|a"};
            yield return new object[] {1.1, "double|1.1"};
            yield return new object[] {1.2f, "float|1.2"};
            yield return new object[] {new byte[]{ 13, 15, 17, 44, 35}, "byteArray|DQ8RLCM=" };
            yield return new object[] {true, "bool|True" };
        }

        [Theory]
        [MemberData(nameof(GenerateData))]
        public void SerializeDistributedContext_should_return_stringValue_by_obj_from_context(object obj, string stringValue)
        {
            const string key = "key";
            Context.Properties.SetProperty(key, obj);
            Context.Configuration.DistributedProperties.Add(key);

            var distributedContext = Context.SerializeDistributedProperties().ToArray();

            distributedContext.Length.Should().Be(1);
            distributedContext[0].Key.Should().Be(key);
            distributedContext[0].Value.Should().Be(stringValue);
        }

        [Fact]
        public void SerializeDistributedContext_should_not_return_stringValue_by_value_equals_null()
        {
            const string key = "key";
            var value = (string) null;
            Context.Properties.SetProperty(key, value);
            Context.Configuration.DistributedProperties.Add(key);

            var distributedContext = Context.SerializeDistributedProperties().ToArray();

            distributedContext.Length.Should().Be(0);
        }

        [Theory]
        [MemberData(nameof(GenerateData))]
        public void PopulateDistributedProperties_should_set_obj_in_context_by_stringValue(object obj, string stringValue)
        {
            const string key = "key";
            var distributedContext = new[]
            {
                new KeyValuePair<string, string>(key, stringValue)
            };
            Context.Configuration.DistributedProperties.Add(key);

            Context.PopulateDistributedProperties(distributedContext);

            Context.Properties.Current[key].ShouldBeEquivalentTo(obj);
        }

        [Fact]
        public void GenerateData_should_contain_all_serialized_types()
        {
            var serializedTypes = GetAllTypedSerializers()
                .Select(x => x.Type)
                .ToArray();

            var testingTypes = GenerateData()
                .Select(x => x[0])
                .Where(x => x != null)
                .Select(x => x.GetType())
                .Distinct()
                .ToArray();

            testingTypes.Should().Contain(serializedTypes);
        }

        private static ITypedSerializer[] GetAllTypedSerializers()
        {
            return typeof(ITypedSerializer).Assembly
                .GetTypes()
                .Where(type => !type.IsAbstract && type.IsClass)
                .Where(type => typeof(ITypedSerializer).IsAssignableFrom(type))
                .Select(Activator.CreateInstance)
                .OfType<ITypedSerializer>()
                .ToArray();
        }
    }
}