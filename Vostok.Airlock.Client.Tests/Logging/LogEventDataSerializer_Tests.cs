using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vostok.Airlock.Logging;
using Vostok.Logging;

namespace Vostok.Airlock.Client.Tests.Logging
{
    public class LogEventDataSerializer_Tests
    {
        [Test]
        public void SerializeDeserializeTest()
        {
            var logEventData = new LogEventData
            {
                Level = LogLevel.Error,
                Message = "Hello!",
                Properties = new Dictionary<string, string> { ["key"] = "val" },
                Timestamp = DateTimeOffset.UtcNow,
                Exceptions = new List<LogEventException>
                {
                    new LogEventException
                    {
                        Message = "msg",
                        Module = "MyModule",
                        Type = "ExType",
                        Stack = new List<LogEventStackFrame>
                        {
                            new LogEventStackFrame
                            {
                                Module = "Mod1",
                                ColumnNumber = 12,
                                Filename = "my.cs",
                                Function = "hello",
                                LineNumber = 2,
                                Source = "hello()"
                            }
                        }

                    }
                }
            };
            GenericSerializeDeserializeTest.RunTest<LogEventData, LogEventDataSerializer>(logEventData);
        }
    }
}