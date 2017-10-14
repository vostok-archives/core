using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Modules;
using Vostok.Clusterclient.Ordering.Storage;
using NUnit.Framework;

namespace Vostok.Clusterclient.Core.Modules
{
    public class RequestModuleChainBuilder_Tests
    {
        private readonly IRequestModule module1;
        private readonly IRequestModule module2;
        private readonly IRequestModule module3;
        private readonly IRequestModule module4;
        private readonly IRequestContext context;
        private readonly List<IRequestModule> calledModules;

        public RequestModuleChainBuilder_Tests()
        {
            module1 = Substitute.For<IRequestModule>();
            module2 = Substitute.For<IRequestModule>();
            module3 = Substitute.For<IRequestModule>();
            module4 = Substitute.For<IRequestModule>();

            calledModules = new List<IRequestModule>();

            module1.ExecuteAsync(null, null)
                .ReturnsForAnyArgs(
                    info =>
                    {
                        calledModules.Add(module1);
                        return info.Arg<Func<IRequestContext, Task<ClusterResult>>>()(info.Arg<IRequestContext>());
                    });

            module2.ExecuteAsync(null, null)
                .ReturnsForAnyArgs(
                    info =>
                    {
                        calledModules.Add(module2);
                        return info.Arg<Func<IRequestContext, Task<ClusterResult>>>()(info.Arg<IRequestContext>());
                    });

            module3.ExecuteAsync(null, null)
                .ReturnsForAnyArgs(
                    info =>
                    {
                        calledModules.Add(module3);
                        return info.Arg<Func<IRequestContext, Task<ClusterResult>>>()(info.Arg<IRequestContext>());
                    });

            module4.ExecuteAsync(null, null)
                .ReturnsForAnyArgs(
                    _ =>
                    {
                        calledModules.Add(module4);
                        return Task.FromResult(ClusterResult.UnexpectedException(context.Request));
                    });

            context = Substitute.For<IRequestContext>();
        }

        [Test]
        public void Should_build_a_delegate_which_invokes_all_modules_in_correct_order()
        {
            var chainDelegate = RequestModuleChainBuilder.BuildChainDelegate(new[] {module1, module2, module3, module4});

            chainDelegate(context).Result.Status.Should().Be(ClusterResultStatus.UnexpectedException);

            calledModules.Should().Equal(module1, module2, module3, module4);
        }

        [Test]
        public void Should_build_a_delegate_which_checks_cancellation_token_before_each_module_invocation()
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            tokenSource.Cancel();

            context.CancellationToken.Returns(token);

            var chainDelegate = RequestModuleChainBuilder.BuildChainDelegate(new[] {module1, module2, module3, module4});

            chainDelegate(context).Result.Status.Should().Be(ClusterResultStatus.Canceled);

            calledModules.Should().BeEmpty();
        }

        [Test]
        public void Should_build_a_chain_with_correct_modules_composition_and_disposition()
        {
            var configuration = Substitute.For<IClusterClientConfiguration>();

            configuration.Modules.Returns(new List<IRequestModule> {module1, module2});

            var storageProvider = Substitute.For<IReplicaStorageProvider>();

            var modules = RequestModuleChainBuilder.BuildChain(configuration, storageProvider);

            modules.Should().HaveCount(13);

            modules[0].Should().BeOfType<ErrorCatchingModule>();
            modules[1].Should().BeOfType<RequestTransformationModule>();
            modules[2].Should().BeOfType<RequestPriorityApplicationModule>();
            modules[3].Should().BeSameAs(module1);
            modules[4].Should().BeSameAs(module2);
            modules[5].Should().BeOfType<LoggingModule>();
            modules[6].Should().BeOfType<ResponseTransformationModule>();
            modules[7].Should().BeOfType<ErrorCatchingModule>();
            modules[8].Should().BeOfType<RequestValidationModule>();
            modules[9].Should().BeOfType<TimeoutValidationModule>();
            modules[10].Should().BeOfType<RequestRetryModule>();
            modules[11].Should().BeOfType<AbsoluteUrlSenderModule>();
            modules[12].Should().BeOfType<RequestExecutionModule>();
        }

        [Test]
        public void Should_build_a_chain_with_correct_modules_composition_and_disposition_when_optional_modules_are_enabled()
        {
            var configuration = Substitute.For<IClusterClientConfiguration>();

            configuration.Modules.Returns(null as List<IRequestModule>);
            configuration.AdaptiveThrottling.Returns(new AdaptiveThrottlingOptions("foo"));
            configuration.ReplicaBudgeting.Returns(new ReplicaBudgetingOptions("foo"));

            var storageProvider = Substitute.For<IReplicaStorageProvider>();

            var modules = RequestModuleChainBuilder.BuildChain(configuration, storageProvider);

            modules.Should().HaveCount(13);

            modules[0].Should().BeOfType<ErrorCatchingModule>();
            modules[1].Should().BeOfType<RequestTransformationModule>();
            modules[2].Should().BeOfType<RequestPriorityApplicationModule>();
            modules[3].Should().BeOfType<LoggingModule>();
            modules[4].Should().BeOfType<ResponseTransformationModule>();
            modules[5].Should().BeOfType<ErrorCatchingModule>();
            modules[6].Should().BeOfType<RequestValidationModule>();
            modules[7].Should().BeOfType<TimeoutValidationModule>();
            modules[8].Should().BeOfType<RequestRetryModule>();
            modules[9].Should().BeOfType<AdaptiveThrottlingModule>();
            modules[10].Should().BeOfType<ReplicaBudgetingModule>();
            modules[11].Should().BeOfType<AbsoluteUrlSenderModule>();
            modules[12].Should().BeOfType<RequestExecutionModule>();
        }
    }
}
