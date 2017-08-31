using FluentAssertions;
using NSubstitute;
using Vostok.Clusterclient.Criteria;
using Vostok.Clusterclient.Helpers;
using Vostok.Clusterclient.Modules;
using Vostok.Clusterclient.Transforms;
using Xunit;
using Xunit.Abstractions;

namespace Vostok.Clusterclient.Core
{
    public class ClusterClientConfigurationExtensions_Tests
    {
        private readonly ClusterClientConfiguration configuration;

        public ClusterClientConfigurationExtensions_Tests(ITestOutputHelper outputHelper)
        {
            configuration = new ClusterClientConfiguration(new TestOutputLog(outputHelper));
        }

        [Fact]
        public void SetupResponseCriteria_should_build_correct_criteria_list()
        {
            var criterion1 = Substitute.For<IResponseCriterion>();
            var criterion2 = Substitute.For<IResponseCriterion>();

            configuration.SetupResponseCriteria(criterion1, criterion2);

            configuration.ResponseCriteria.Should().Equal(criterion1, criterion2);
        }

        [Fact]
        public void AddRequestMoudle_should_not_fail_if_modules_list_is_null()
        {
            configuration.Modules = null;

            configuration.AddRequestModule(Substitute.For<IRequestModule>());

            configuration.Modules.Should().HaveCount(1);
        }

        [Fact]
        public void AddRequestTransform_should_not_fail_if_transforms_list_is_null()
        {
            configuration.RequestTransforms = null;

            configuration.AddRequestTransform(Substitute.For<IRequestTransform>());

            configuration.RequestTransforms.Should().HaveCount(1);
        }

        [Fact]
        public void AddResponseTransform_should_not_fail_if_transforms_list_is_null()
        {
            configuration.ResponseTransforms = null;

            configuration.AddResponseTransform(Substitute.For<IResponseTransform>());

            configuration.ResponseTransforms.Should().HaveCount(1);
        }
    }
}
