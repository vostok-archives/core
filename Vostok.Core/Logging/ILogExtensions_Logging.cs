using System;

namespace Vostok.Logging
{
    public static class ILogExtensions_Logging
    {
        public static void Info(this ILog log, string message)
        {
            log.Log(LogLevel.Info, null, message, Array.Empty<object>());
        }

        public static void Info(this ILog log, Exception exception)
        {
            log.Log(LogLevel.Info, exception, string.Empty, Array.Empty<object>());
        }

        public static void Info(this ILog log, string messageTemplate, params object[] parameters)
        {
            log.Log(LogLevel.Info, null, messageTemplate, parameters);
        }

        public static void Info(this ILog log, string message, Exception exception)
        {
            log.Log(LogLevel.Info, exception, message, Array.Empty<object>());
        }

        public static void Info(this ILog log, Exception exception, string message)
        {
            log.Log(LogLevel.Info, exception, message, Array.Empty<object>());
        }

        public static void Info(this ILog log, Exception exception, string messageTemplate, params object[] parameters)
        {
            log.Log(LogLevel.Info, exception, messageTemplate, parameters);
        }

        public static void Error(this ILog log, string message)
        {
            log.Log(LogLevel.Error, null, message, Array.Empty<object>());
        }

        public static void Error(this ILog log, Exception exception)
        {
            log.Log(LogLevel.Error, exception, string.Empty, Array.Empty<object>());
        }

        public static void Error(this ILog log, string messageTemplate, params object[] parameters)
        {
            log.Log(LogLevel.Error, null, messageTemplate, parameters);
        }

        public static void Error(this ILog log, string message, Exception exception)
        {
            log.Log(LogLevel.Error, exception, message, Array.Empty<object>());
        }

        public static void Error(this ILog log, Exception exception, string message)
        {
            log.Log(LogLevel.Error, exception, message, Array.Empty<object>());
        }

        public static void Error(this ILog log, Exception exception, string messageTemplate, params object[] parameters)
        {
            log.Log(LogLevel.Error, exception, messageTemplate, parameters);
        }

        public static void Fatal(this ILog log, string message)
        {
            log.Log(LogLevel.Fatal, null, message, Array.Empty<object>());
        }

        public static void Fatal(this ILog log, Exception exception)
        {
            log.Log(LogLevel.Fatal, exception, string.Empty, Array.Empty<object>());
        }

        public static void Fatal(this ILog log, string messageTemplate, params object[] parameters)
        {
            log.Log(LogLevel.Fatal, null, messageTemplate, parameters);
        }

        public static void Fatal(this ILog log, string message, Exception exception)
        {
            log.Log(LogLevel.Fatal, exception, message, Array.Empty<object>());
        }

        public static void Fatal(this ILog log, Exception exception, string message)
        {
            log.Log(LogLevel.Fatal, exception, message, Array.Empty<object>());
        }

        public static void Fatal(this ILog log, Exception exception, string messageTemplate, params object[] parameters)
        {
            log.Log(LogLevel.Fatal, exception, messageTemplate, parameters);
        }

        public static void Debug(this ILog log, string message)
        {
            log.Log(LogLevel.Debug, null, message, Array.Empty<object>());
        }

        public static void Debug(this ILog log, Exception exception)
        {
            log.Log(LogLevel.Debug, exception, string.Empty, Array.Empty<object>());
        }

        public static void Debug(this ILog log, string messageTemplate, params object[] parameters)
        {
            log.Log(LogLevel.Debug, null, messageTemplate, parameters);
        }

        public static void Debug(this ILog log, string message, Exception exception)
        {
            log.Log(LogLevel.Debug, exception, message, Array.Empty<object>());
        }

        public static void Debug(this ILog log, Exception exception, string message)
        {
            log.Log(LogLevel.Debug, exception, message, Array.Empty<object>());
        }

        public static void Debug(this ILog log, Exception exception, string messageTemplate, params object[] parameters)
        {
            log.Log(LogLevel.Debug, exception, messageTemplate, parameters);
        }

        public static void Trace(this ILog log, string message)
        {
            log.Log(LogLevel.Trace, null, message, Array.Empty<object>());
        }

        public static void Trace(this ILog log, Exception exception)
        {
            log.Log(LogLevel.Trace, exception, string.Empty, Array.Empty<object>());
        }

        public static void Trace(this ILog log, string messageTemplate, params object[] parameters)
        {
            log.Log(LogLevel.Trace, null, messageTemplate, parameters);
        }

        public static void Trace(this ILog log, string message, Exception exception)
        {
            log.Log(LogLevel.Trace, exception, message, Array.Empty<object>());
        }

        public static void Trace(this ILog log, Exception exception, string message)
        {
            log.Log(LogLevel.Trace, exception, message, Array.Empty<object>());
        }

        public static void Trace(this ILog log, Exception exception, string messageTemplate, params object[] parameters)
        {
            log.Log(LogLevel.Trace, exception, messageTemplate, parameters);
        }

        public static void Warn(this ILog log, string message)
        {
            log.Log(LogLevel.Warn, null, message, Array.Empty<object>());
        }

        public static void Warn(this ILog log, Exception exception)
        {
            log.Log(LogLevel.Warn, exception, string.Empty, Array.Empty<object>());
        }

        public static void Warn(this ILog log, string messageTemplate, params object[] parameters)
        {
            log.Log(LogLevel.Warn, null, messageTemplate, parameters);
        }

        public static void Warn(this ILog log, string message, Exception exception)
        {
            log.Log(LogLevel.Warn, exception, message, Array.Empty<object>());
        }

        public static void Warn(this ILog log, Exception exception, string message)
        {
            log.Log(LogLevel.Warn, exception, message, Array.Empty<object>());
        }

        public static void Warn(this ILog log, Exception exception, string messageTemplate, params object[] parameters)
        {
            log.Log(LogLevel.Warn, exception, messageTemplate, parameters);
        }

        public static void Log(this ILog log, LogLevel level, Exception exception, string message, params object[] parameters)
        {
            log.Log(new LogEvent(level, exception, message, parameters));
        }
    }
}