using System;
using Vostok.Logging;
using Xunit.Abstractions;

namespace Vostok.Clusterclient.Helpers
{
    internal class TestOutputLog : ILog
    {
        private readonly ITestOutputHelper outputHelper;

        public TestOutputLog(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
        }

        public void Log(LogEvent logEvent)
        {
            outputHelper.WriteLine($"{DateTime.Now:T} {logEvent.Level} {string.Format(logEvent.MessageTemplate, logEvent.MessageParameters)} {logEvent.Exception}");
        }

        public bool IsEnabledFor(LogLevel level)
        {
            return true;
        }
    }
}