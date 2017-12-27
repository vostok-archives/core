using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Vostok.Airlock.Logging
{
    public sealed class LogEventException
    {
        public LogEventException()
        {
            
        }

        public LogEventException(Exception ex)
        {
            if (ex == null)
                return;
            Module = ex.Source;
            Message = ex.Message;
            Type = ex.GetType().FullName;
            var frames = new StackTrace(ex, true).GetFrames();
            if (frames == null)
                return;
            Stack = new List<LogEventStackFrame>();
            foreach (var frame in frames)
            {
                Stack.Add(new LogEventStackFrame(frame));
            }
        }

        public string Message { get; set; }
        public string Type { get; set; }
        public string Module { get; set; }
        public List<LogEventStackFrame> Stack { get; set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            if (Type != null)
                stringBuilder.Append(Type);
            if (Message != null)
            {
                if (stringBuilder.Length > 0)
                    stringBuilder.Append(": ");
                stringBuilder.Append(Message);
                stringBuilder.AppendLine();
            }
            if (Stack != null)
            {
                foreach (var stackFrame in Stack)
                {
                    stringBuilder.Append("   at ");
                    stringBuilder.Append(stackFrame);
                    stringBuilder.AppendLine();
                }
            }
            return stringBuilder.ToString().TrimEnd();
        }
    }
}