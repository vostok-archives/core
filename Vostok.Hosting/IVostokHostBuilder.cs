using System;
using Microsoft.Extensions.Configuration;
using Vostok.Hosting.Configuration;

namespace Vostok.Hosting
{
    public interface IVostokHostBuilder
    {
        IVostokHost Build();
        string GetSetting(string key);
        IVostokHostBuilder UseSetting(string key, string value);
        IVostokHostBuilder ConfigureAppConfiguration(Action<VostokHostBuilderContext, IConfigurationBuilder> configureDelegate);
        IVostokHostBuilder ConfigureHost(Action<VostokHostBuilderContext, IHostConfigurator> configureDelegate);
        IVostokHostBuilder ConfigureAirlock(Action<VostokHostBuilderContext, IAirlockConfigurator> configureDelegate);
        IVostokHostBuilder ConfigureTracing(Action<VostokHostBuilderContext, ITracingConfigurator> configureDelegate);
        IVostokHostBuilder ConfigureMetrics(Action<VostokHostBuilderContext, IMetricsConfigurator> configureDelegate);
    }
}