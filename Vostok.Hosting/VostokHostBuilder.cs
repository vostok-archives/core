using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Vostok.Airlock;
using Vostok.Airlock.Metrics;
using Vostok.Clusterclient.Topology;
using Vostok.Commons.Model;
using Vostok.Commons.Utilities;
using Vostok.Hosting.Configuration;
using Vostok.Logging;
using Vostok.Logging.Logs;
using Vostok.Metrics;
using Vostok.Tracing;

namespace Vostok.Hosting
{
    public class VostokHostBuilder : IVostokHostBuilder
    {
        private readonly VostokHostingEnvironment hostingEnvironment;
        private readonly VostokHostBuilderContext context;
        private readonly IConfiguration config;

        private readonly List<Action<VostokHostBuilderContext, IConfigurationBuilder>> configureAppConfigurationDelegates = new List<Action<VostokHostBuilderContext, IConfigurationBuilder>>();
        private readonly List<Action<VostokHostBuilderContext, IHostConfigurator>> configureHostDelegates = new List<Action<VostokHostBuilderContext, IHostConfigurator>>();
        private readonly List<Action<VostokHostBuilderContext, IAirlockConfigurator>> configureAirlockDelegates = new List<Action<VostokHostBuilderContext, IAirlockConfigurator>>();
        private readonly List<Action<VostokHostBuilderContext, ITracingConfigurator>> configureTracingDelegates = new List<Action<VostokHostBuilderContext, ITracingConfigurator>>();
        private readonly List<Action<VostokHostBuilderContext, IMetricsConfigurator>> configureMetricsDelegates = new List<Action<VostokHostBuilderContext, IMetricsConfigurator>>();
        private readonly List<Action<VostokHostBuilderContext, IApplicationConfigurator>> configureApplicationDelegates = new List<Action<VostokHostBuilderContext, IApplicationConfigurator>>();
        private bool hostBuilt;

        public VostokHostBuilder()
        {
            hostingEnvironment = new VostokHostingEnvironment
            {
                HostLog = new ConsoleLog()
            };
            config = new ConfigurationBuilder().AddEnvironmentVariables("VOSTOK_").Build();
            context = new VostokHostBuilderContext
            {
                Configuration = config
            };
        }

        public IVostokHostBuilder SetServiceInfo(string project, string service)
        {
            hostingEnvironment.Project = project;
            hostingEnvironment.Service = service;
            return this;
        }

        public string GetSetting(string key)
        {
            return config[key];
        }

        public IVostokHostBuilder UseSetting(string key, string value)
        {
            config[key] = value;
            return this;
        }

        public IVostokHost Build()
        {
            if (hostBuilt)
                throw new InvalidOperationException("Vostok host is already built");
            hostBuilt = true;
            try
            {
                if (string.IsNullOrEmpty(hostingEnvironment.Project))
                    throw new InvalidOperationException($"Project name was not set. Use {nameof(SetServiceInfo)} method");
                if (string.IsNullOrEmpty(hostingEnvironment.Service))
                    throw new InvalidOperationException($"Service name was not set. Use {nameof(SetServiceInfo)} method");

                context.HostingEnvironment = hostingEnvironment;
                var configurationBuilder = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddInMemoryCollection(config.AsEnumerable());
                foreach (var configurationDelegate in configureAppConfigurationDelegates)
                    configurationDelegate(context, configurationBuilder);
                var configurationRoot = configurationBuilder.Build();
                context.Configuration = configurationRoot;

                hostingEnvironment.Configuration = configurationRoot;
                hostingEnvironment.Environment = configurationRoot[VostokConfigurationDefaults.EnvironmentKey];
                var hostConfigurator = new HostConfigurator(hostingEnvironment);
                foreach (var configurationDelegate in configureHostDelegates)
                    configurationDelegate(context, hostConfigurator);
                if (string.IsNullOrEmpty(hostingEnvironment.Environment))
                    hostingEnvironment.Environment = VostokEnvironmentNames.Production;

                var airlockConfigurator = new AirlockConfigurator();
                foreach (var configurationDelegate in configureAirlockDelegates)
                    configurationDelegate(context, airlockConfigurator);
                var airlockConfig = airlockConfigurator.AirlockConfig ?? ReadAirlockConfig();
                var parallelizm = airlockConfigurator.Parallelism ?? ReadAirlockParallelism() ?? VostokConfigurationDefaults.DefaultAirlockParallelism;
                hostingEnvironment.AirlockClient = CreateAirlockClient(airlockConfig, parallelizm, airlockConfigurator.Log);

                DefaultConfigureTracing(configurationRoot);
                var tracingConfigurator = new TracingConfigurator();
                foreach (var configurationDelegate in configureTracingDelegates)
                    configurationDelegate(context, tracingConfigurator);

                var metricConfiguration = new MetricConfiguration();
                DefaultConfigureMetrics(configurationRoot, metricConfiguration);
                var metricsConfigurator = new MetricsConfigurator(metricConfiguration);
                foreach (var configurationDelegate in configureMetricsDelegates)
                    configurationDelegate(context, metricsConfigurator);
                metricConfiguration.Reporter = new AirlockMetricReporter(hostingEnvironment.AirlockClient, RoutingKey.CreatePrefix(hostingEnvironment.Project, hostingEnvironment.Environment, hostingEnvironment.Service));
                hostingEnvironment.MetricScope = new RootMetricScope(metricConfiguration);

                var applicationDelegateBuilder = new ApplicationDelegateBuilder();
                foreach (var configureDelegate in configureApplicationDelegates)
                    configureDelegate(context, applicationDelegateBuilder);

                return new VostokHost(hostingEnvironment, applicationDelegateBuilder.OnStartAsync);
            }
            catch (Exception e)
            {
                return new VostokHost(hostingEnvironment, async _ => throw new AggregateException("Failed to build host", e));
            }
        }

        public IVostokHostBuilder ConfigureAppConfiguration(Action<VostokHostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            if (configureDelegate == null)
                throw new ArgumentNullException(nameof(configureDelegate));
            configureAppConfigurationDelegates.Add(configureDelegate);
            return this;
        }

        public IVostokHostBuilder ConfigureHost(Action<VostokHostBuilderContext, IHostConfigurator> configureDelegate)
        {
            if (configureDelegate == null)
                throw new ArgumentNullException(nameof(configureDelegate));
            configureHostDelegates.Add(configureDelegate);
            return this;
        }

        public IVostokHostBuilder ConfigureAirlock(Action<VostokHostBuilderContext, IAirlockConfigurator> configureDelegate)
        {
            if (configureDelegate == null)
                throw new ArgumentNullException(nameof(configureDelegate));
            configureAirlockDelegates.Add(configureDelegate);
            return this;
        }

        public IVostokHostBuilder ConfigureTracing(Action<VostokHostBuilderContext, ITracingConfigurator> configureDelegate)
        {
            if (configureDelegate == null)
                throw new ArgumentNullException(nameof(configureDelegate));
            configureTracingDelegates.Add(configureDelegate);
            return this;
        }

        public IVostokHostBuilder ConfigureMetrics(Action<VostokHostBuilderContext, IMetricsConfigurator> configureDelegate)
        {
            if (configureDelegate == null)
                throw new ArgumentNullException(nameof(configureDelegate));
            configureMetricsDelegates.Add(configureDelegate);
            return this;
        }

        public IVostokHostBuilder ConfigureApplication(Action<VostokHostBuilderContext, IApplicationConfigurator> configureDelegate)
        {
            if (configureDelegate == null)
                throw new ArgumentNullException(nameof(configureDelegate));
            configureApplicationDelegates.Add(configureDelegate);
            return this;
        }

        private static void DefaultConfigureTracing(IConfiguration configuration)
        {
            var tracingSection = configuration.GetSection(VostokConfigurationDefaults.TracingSection);
            Trace.Configuration.ContextFieldsWhitelist.UnionWith((tracingSection[nameof(Trace.Configuration.ContextFieldsWhitelist)] ?? "").Split(new[] {' ', ';', ','}, StringSplitOptions.RemoveEmptyEntries));
            Trace.Configuration.InheritedFieldsWhitelist.UnionWith((tracingSection[nameof(Trace.Configuration.InheritedFieldsWhitelist)] ?? "").Split(new[] {' ', ';', ','}, StringSplitOptions.RemoveEmptyEntries));
        }

        private static void DefaultConfigureMetrics(IConfiguration configuration, IMetricConfiguration metricConfiguration)
        {
            var metricsSection = configuration.GetSection(VostokConfigurationDefaults.MetricsSection);
            metricConfiguration.ContextFieldsWhitelist.UnionWith((metricsSection[nameof(metricConfiguration.ContextFieldsWhitelist)] ?? "").Split(new[] {' ', ';', ','}, StringSplitOptions.RemoveEmptyEntries));
        }

        private static IAirlockClient CreateAirlockClient(AirlockConfig airlockConfig, int parallelism, ILog log)
        {
            if (airlockConfig?.ApiKey == null || airlockConfig.ClusterProvider == null)
                return null;
            if (parallelism <= 1)
                return new AirlockClient(airlockConfig, log);
            return new ParallelAirlockClient(airlockConfig, parallelism, log);
        }

        private int? ReadAirlockParallelism()
        {
            var airlockSection = context.Configuration.GetSection(VostokConfigurationDefaults.AirlockSection);
            var value = airlockSection[VostokConfigurationDefaults.AirlockParallelismKey];
            if (string.IsNullOrEmpty(value))
                return null;
            if (!int.TryParse(value, out var result))
                throw new InvalidOperationException($"Invalid value '{value}' for {VostokConfigurationDefaults.AirlockSection}{ConfigurationPath.KeyDelimiter}{VostokConfigurationDefaults.AirlockParallelismKey}");
            return result;
        }

        private AirlockConfig ReadAirlockConfig()
        {
            var airlockSection = context.Configuration.GetSection(VostokConfigurationDefaults.AirlockSection);
            var host = airlockSection[VostokConfigurationDefaults.HostKey];
            if (string.IsNullOrEmpty(host))
                return null;
            var airlockConfig = new AirlockConfig
            {
                ClusterProvider = new FixedClusterProvider(new Uri(host))
            };
            foreach (var propertyInfo in typeof (AirlockConfig).GetProperties())
            {
                var value = airlockSection[propertyInfo.Name];
                if (!string.IsNullOrEmpty(value))
                {
                    if (propertyInfo.PropertyType == typeof (string))
                        propertyInfo.SetValue(airlockConfig, value);
                    else if (propertyInfo.PropertyType == typeof (int))
                    {
                        if (!int.TryParse(value, out var intValue))
                            throw new InvalidOperationException($"Invalid value '{value}' for vostok.airlock.{propertyInfo.Name}");
                        propertyInfo.SetValue(airlockConfig, intValue);
                    }
                    else if (propertyInfo.PropertyType == typeof (DataSize))
                    {
                        if (!DataSize.TryParse(value, out var dataSizeValue))
                            throw new InvalidOperationException($"Invalid value '{value}' for vostok.airlock.{propertyInfo.Name}");
                        propertyInfo.SetValue(airlockConfig, dataSizeValue);
                    }
                    else if (propertyInfo.PropertyType == typeof (TimeSpan))
                    {
                        if (!DurationParser.TryParse(value, out var durationValue))
                            throw new InvalidOperationException($"Invalid value '{value}' for vostok.airlock.{propertyInfo.Name}");
                        propertyInfo.SetValue(airlockConfig, durationValue);
                    }
                    else if (propertyInfo.PropertyType == typeof (bool))
                    {
                        if (!bool.TryParse(value, out var boolValue))
                            throw new InvalidOperationException($"Invalid value '{value}' for vostok.airlock.{propertyInfo.Name}");
                        propertyInfo.SetValue(airlockConfig, boolValue);
                    }
                }
            }
            return airlockConfig;
        }

        private class ApplicationDelegateBuilder : IApplicationConfigurator
        {
            public StartServiceDelegate OnStartAsync { get; private set; }

            public void OnStart(StartServiceDelegate onStartAsync)
            {
                OnStartAsync = onStartAsync;
            }
        }

        private class TracingConfigurator : ITracingConfigurator
        {
            public void AddContextFieldswhitelist(params string[] fields)
            {
                Trace.Configuration.ContextFieldsWhitelist.UnionWith(fields);
            }

            public void AddInheritedFieldswhitelist(params string[] fields)
            {
                Trace.Configuration.InheritedFieldsWhitelist.UnionWith(fields);
            }
        }

        private class MetricsConfigurator : IMetricsConfigurator
        {
            private readonly IMetricConfiguration metricConfiguration;

            public MetricsConfigurator(IMetricConfiguration metricConfiguration)
            {
                this.metricConfiguration = metricConfiguration;
            }

            public void AddContextFieldswhitelist(params string[] fields)
            {
                metricConfiguration.ContextFieldsWhitelist.UnionWith(fields);
            }
        }

        private class AirlockConfigurator : IAirlockConfigurator
        {
            public AirlockConfig AirlockConfig { get; private set; }
            public int? Parallelism { get; private set; }
            public ILog Log { get; private set; }

            public void SetConfig(AirlockConfig airlockConfig)
            {
                AirlockConfig = airlockConfig;
            }

            public void SetParallelism(int parallelism)
            {
                Parallelism = parallelism;
            }

            public void SetLog(ILog log)
            {
                Log = log;
            }
        }

        private class HostConfigurator : IHostConfigurator
        {
            private readonly VostokHostingEnvironment hostingEnvironment;

            public HostConfigurator(VostokHostingEnvironment hostingEnvironment)
            {
                this.hostingEnvironment = hostingEnvironment;
            }

            public void SetEnvironment(string environment)
            {
                hostingEnvironment.Environment = environment;
            }

            public void SetHostLog(ILog log)
            {
                hostingEnvironment.HostLog = log;
            }
        }
    }
}