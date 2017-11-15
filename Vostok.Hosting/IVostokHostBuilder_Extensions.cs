using System;
using Microsoft.Extensions.Configuration;
using Vostok.Hosting.Configuration;

namespace Vostok.Hosting
{
    public static class IVostokHostBuilder_Extensions
    {
        public static IVostokHostBuilder ConfigureAppConfiguration(this IVostokHostBuilder hostBuilder, Action<IConfigurationBuilder> configureDelegate)
        {
            return hostBuilder.ConfigureAppConfiguration((_, configurationBuilder) => configureDelegate(configurationBuilder));
        }

        public static IVostokHostBuilder ConfigureHost(this IVostokHostBuilder hostBuilder, Action<IHostConfigurator> configureDelegate)
        {
            return hostBuilder.ConfigureHost((_, hostConfigurator) => configureDelegate(hostConfigurator));
        }

        public static IVostokHostBuilder ConfigureAirlock(this IVostokHostBuilder hostBuilder, Action<IAirlockConfigurator> configureDelegate)
        {
            return hostBuilder.ConfigureAirlock((_, airlockConfigurator) => configureDelegate(airlockConfigurator));
        }

        public static IVostokHostBuilder ConfigureTracing(this IVostokHostBuilder hostBuilder, Action<ITracingConfigurator> configureDelegate)
        {
            return hostBuilder.ConfigureTracing((_, tracingConfigurator) => configureDelegate(tracingConfigurator));
        }

        public static IVostokHostBuilder ConfigureMetrics(this IVostokHostBuilder hostBuilder, Action<IMetricsConfigurator> configureDelegate)
        {
            return hostBuilder.ConfigureMetrics((_, metricsConfigurator) => configureDelegate(metricsConfigurator));
        }
    }
}