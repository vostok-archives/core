using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using Serilog.Events;
using SerilogEvent = Serilog.Events.LogEvent;
using SerilogLevel = Serilog.Events.LogEventLevel;

namespace Vostok.Logging.Serilog
{
    public sealed class SerilogLog : ILog
    {
        private readonly ILogger logger;

        public SerilogLog(ILogger logger)
        {
            this.logger = logger;
        }

        public void Log(LogEvent logEvent)
        {
            var level = TranslateLevel(logEvent.Level);
            if (!logger.IsEnabled(level))
            {
                return;
            }

            var timestamp = DateTimeOffset.Now;
            var exception = logEvent.Exception;
            var template = BindTemplate(logEvent.MessageTemplate, logEvent.MessageParameters);
            var properties = BindProperties(logEvent.Properties);
            logger.Write(new SerilogEvent(timestamp, level, exception, template.MessageTemplate, template.Parameters.Concat(properties)));
        }

        public bool IsEnabledFor(LogLevel level)
        {
            return logger.IsEnabled(TranslateLevel(level));
        }

        private IEnumerable<LogEventProperty> BindProperties(IReadOnlyDictionary<string, object> properties)
        {
            var bindedProperties = new List<LogEventProperty>(properties.Count);
            foreach (var property in properties)
            {
                LogEventProperty bindedProperty;
                if (logger.BindProperty(property.Key, property.Value, false, out bindedProperty))
                {
                    bindedProperties.Add(bindedProperty);
                }
            }
            return bindedProperties;
        }

        private Template BindTemplate(string messageTemplate, object[] parameters)
        {
            MessageTemplate bindedMessageTemplate;
            IEnumerable<LogEventProperty> bindedParameters;
            return logger.BindMessageTemplate(messageTemplate, parameters, out bindedMessageTemplate, out bindedParameters)
                ? new Template(bindedMessageTemplate, bindedParameters)
                : Template.Empty;
        }

        private SerilogLevel TranslateLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return SerilogLevel.Verbose;
                case LogLevel.Debug:
                    return SerilogLevel.Debug;
                case LogLevel.Info:
                    return SerilogLevel.Information;
                case LogLevel.Warn:
                    return SerilogLevel.Warning;
                case LogLevel.Error:
                    return SerilogLevel.Error;
                case LogLevel.Fatal:
                    return SerilogLevel.Fatal;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        private struct Template
        {
            public static readonly Template Empty = new Template(MessageTemplate.Empty, Array.Empty<LogEventProperty>()); 

            public Template(MessageTemplate messageTemplate, IEnumerable<LogEventProperty> parameters)
            {
                MessageTemplate = messageTemplate;
                Parameters = parameters;
            }

            public MessageTemplate MessageTemplate { get; }

            public IEnumerable<LogEventProperty> Parameters { get; }
        }
    }
}