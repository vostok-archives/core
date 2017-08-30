using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Vostok.Flow
{
    public class ContextPropertiesSnapshot_Tests
    {
        [Fact]
        public void Empty_instance_should_not_contain_any_properties()
        {
            ContextPropertiesSnapshot.Empty.Should().BeEmpty();
        }

        [Fact]
        public void Empty_instance_should_have_zero_count()
        {
            ContextPropertiesSnapshot.Empty.Count.Should().Be(0);
        }

        [Fact]
        public void Should_be_able_to_add_a_property_to_empty_instance()
        {
            var properties = ContextPropertiesSnapshot.Empty.Set("k", "v");

            properties.Count.Should().Be(1);
            properties["k"].Should().Be("v");
        }

        [Fact]
        public void Should_not_modify_empty_instance_when_deriving_from_it()
        {
            ContextPropertiesSnapshot.Empty.Set("k", "v");

            ContextPropertiesSnapshot.Empty.Should().BeEmpty();

            ContextPropertiesSnapshot.Empty.Count.Should().Be(0);
        }

        [Fact]
        public void Should_be_case_sensitive_when_comparing_property_names()
        {
            var properties = new ContextPropertiesSnapshot(1)
                .Set("key", "value")
                .Set("KEY", "value");

            properties.Should().HaveCount(2);

            properties.Keys.Should().Equal("key", "KEY");
        }

        [Fact]
        public void Set_should_be_able_to_expand_beyond_initial_capacity()
        {
            var properties = new ContextPropertiesSnapshot(1)
                .Set("k1", "v1")
                .Set("k2", "v2")
                .Set("k3", "v3")
                .Set("k4", "v4")
                .Set("k5", "v5");

            properties.Count.Should().Be(5);

            properties.Keys.Should().Equal("k1", "k2", "k3", "k4", "k5");
        }

        [Fact]
        public void Set_should_return_same_instance_when_replacing_a_property_with_same_value()
        {
            var propertiesBefore = ContextPropertiesSnapshot.Empty
                .Set("k1", "v1")
                .Set("k2", "v2")
                .Set("k3", "v3");

            var propertiesAfter = propertiesBefore.Set("k2", "v2");

            propertiesAfter.Should().BeSameAs(propertiesBefore);
        }

        [Fact]
        public void Set_should_replace_existing_property_when_provided_with_a_different_value()
        {
            var propertiesBefore = ContextPropertiesSnapshot.Empty
                .Set("k1", "v1")
                .Set("k2", "v2")
                .Set("k3", "v3");

            var propertiesAfter = propertiesBefore.Set("k2", "vx");

            propertiesAfter.Should().NotBeSameAs(propertiesBefore);
            propertiesAfter.Count.Should().Be(3);
            propertiesAfter["k1"].Should().Be("v1");
            propertiesAfter["k2"].Should().Be("vx");
            propertiesAfter["k3"].Should().Be("v3");
        }

        [Fact]
        public void Set_should_not_modify_base_instance_when_deriving_from_it_by_replacing_existing_property()
        {
            var propertiesBefore = ContextPropertiesSnapshot.Empty
                .Set("k1", "v1")
                .Set("k2", "v2")
                .Set("k3", "v3");

            var propertiesAfter = propertiesBefore.Set("k2", "vx");

            propertiesAfter.Should().NotBeSameAs(propertiesBefore);
            propertiesBefore.Count.Should().Be(3);
            propertiesBefore["k1"].Should().Be("v1");
            propertiesBefore["k2"].Should().Be("v2");
            propertiesBefore["k3"].Should().Be("v3");
        }

        [Fact]
        public void Set_should_be_able_to_grow_properties_when_adding_unique_names_without_spoiling_base_instance()
        {
            var properties = new ContextPropertiesSnapshot(0);

            for (var i = 0; i < 100; i++)
            {
                var newName = "name-" + i;
                var newValue = "value-" + i;

                var newProperties = properties.Set(newName, newValue);

                newProperties.Should().NotBeSameAs(properties);
                newProperties.Count.Should().Be(i + 1);
                newProperties[newName].Should().Be(newValue);

                properties.Count.Should().Be(i);
                properties.ContainsKey(newName).Should().BeFalse();
                properties = newProperties;
            }
        }

        [Fact]
        public void Set_should_correctly_handle_forking_multiple_instances_from_a_single_base()
        {
            var propertiesBefore = ContextPropertiesSnapshot.Empty
                .Set("k1", "v1")
                .Set("k2", "v2")
                .Set("k3", "v3");

            var propertiesAfter1 = propertiesBefore.Set("k4", "v4-1");
            var propertiesAfter2 = propertiesBefore.Set("k4", "v4-2");

            propertiesBefore.Count.Should().Be(3);
            propertiesBefore.ContainsKey("k4").Should().BeFalse();

            propertiesAfter1.Should().NotBeSameAs(propertiesBefore);
            propertiesAfter1.Count.Should().Be(4);
            propertiesAfter1["k4"].Should().Be("v4-1");

            propertiesAfter2.Should().NotBeSameAs(propertiesBefore);
            propertiesAfter2.Should().NotBeSameAs(propertiesAfter1);
            propertiesAfter2.Count.Should().Be(4);
            propertiesAfter2["k4"].Should().Be("v4-2");
        }

        [Fact]
        public void Remove_should_correctly_remove_first_property()
        {
            var propertiesBefore = ContextPropertiesSnapshot.Empty
                .Set("k1", "v1")
                .Set("k2", "v2")
                .Set("k3", "v3")
                .Set("k4", "v4")
                .Set("k5", "v5");

            var propertiesAfter = propertiesBefore.Remove("k1");

            propertiesAfter.Should().NotBeSameAs(propertiesBefore);
            propertiesAfter.Should().HaveCount(4);
            propertiesAfter.ContainsKey("k1").Should().BeFalse();
        }

        [Fact]
        public void Remove_should_correctly_remove_last_property()
        {
            var propertiesBefore = ContextPropertiesSnapshot.Empty
                .Set("k1", "v1")
                .Set("k2", "v2")
                .Set("k3", "v3")
                .Set("k4", "v4")
                .Set("k5", "v5");

            var propertiesAfter = propertiesBefore.Remove("k5");

            propertiesAfter.Should().NotBeSameAs(propertiesBefore);
            propertiesAfter.Should().HaveCount(4);
            propertiesAfter.ContainsKey("k5").Should().BeFalse();
        }

        [Fact]
        public void Remove_should_correctly_remove_a_property_from_the_middle()
        {
            var propertiesBefore = ContextPropertiesSnapshot.Empty
                .Set("k1", "v1")
                .Set("k2", "v2")
                .Set("k3", "v3")
                .Set("k4", "v4")
                .Set("k5", "v5");

            var propertiesAfter = propertiesBefore.Remove("k3");

            propertiesAfter.Should().NotBeSameAs(propertiesBefore);
            propertiesAfter.Should().HaveCount(4);
            propertiesAfter.ContainsKey("k3").Should().BeFalse();
        }

        [Fact]
        public void Remove_should_correctly_remove_the_only_property_in_collection()
        {
            var propertiesBefore = ContextPropertiesSnapshot.Empty
                .Set("k1", "v1");

            var propertiesAfter = propertiesBefore.Remove("k1");

            propertiesAfter.Should().NotBeSameAs(propertiesBefore);
            propertiesAfter.Count.Should().Be(0);
            propertiesAfter.Should().BeEmpty();
        }

        [Fact]
        public void Remove_should_return_same_instance_when_removing_a_property_which_is_not_present()
        {
            var propertiesBefore = ContextPropertiesSnapshot.Empty
                .Set("k1", "v1")
                .Set("k2", "v2")
                .Set("k3", "v3")
                .Set("k4", "v4")
                .Set("k5", "v5");

            var propertiesAfter = propertiesBefore.Remove("k6");

            propertiesAfter.Should().BeSameAs(propertiesBefore);
        }

        [Fact]
        public void Remove_should_not_spoil_base_insance()
        {
            var propertiesBefore = ContextPropertiesSnapshot.Empty
                .Set("k1", "v1")
                .Set("k2", "v2")
                .Set("k3", "v3")
                .Set("k4", "v4")
                .Set("k5", "v5");

            propertiesBefore.Remove("k1");
            propertiesBefore.Remove("k2");
            propertiesBefore.Remove("k3");
            propertiesBefore.Remove("k4");
            propertiesBefore.Remove("k5");

            propertiesBefore.Should().HaveCount(5);
            propertiesBefore["k1"].Should().Be("v1");
            propertiesBefore["k2"].Should().Be("v2");
            propertiesBefore["k3"].Should().Be("v3");
            propertiesBefore["k4"].Should().Be("v4");
            propertiesBefore["k5"].Should().Be("v5");
        }

        [Fact]
        public void Indexer_should_throw_when_properties_are_empty()
        {
            Action action = () => ContextPropertiesSnapshot.Empty["name"].GetHashCode();

            action.ShouldThrowExactly<KeyNotFoundException>();
        }

        [Fact]
        public void Indexer_should_return_null_when_property_with_given_name_does_not_exist()
        {
            var properties = ContextPropertiesSnapshot.Empty
                .Set("name1", "value1")
                .Set("name2", "value2");

            Action action = () => properties["name3"].GetHashCode();

            action.ShouldThrowExactly<KeyNotFoundException>();
        }

        [Fact]
        public void Indexer_should_return_correct_values_for_existing_property_names()
        {
            var properties = ContextPropertiesSnapshot.Empty
                .Set("name1", "value1")
                .Set("name2", "value2");

            properties["name1"].Should().Be("value1");
            properties["name2"].Should().Be("value2");
        }

        [Fact]
        public void Indexer_should_be_case_sensitive_when_comparing_property_names()
        {
            var properties = ContextPropertiesSnapshot.Empty
                .Set("name", "value1")
                .Set("NAME", "value2");

            properties["name"].Should().Be("value1");
            properties["NAME"].Should().Be("value2");
        }
    }
}
