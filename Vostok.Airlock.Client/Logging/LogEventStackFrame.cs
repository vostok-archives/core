using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Vostok.Airlock.Logging
{
    public sealed class LogEventStackFrame
    {
        private readonly Regex asyncRegex = new Regex(@"^(.*)[\.\+]<(\w*)>d__\d*");
        private readonly Regex lambdaRegex = new Regex(@"^<?(\w*)>b__\w+");
        public LogEventStackFrame()
        {
            
        }

        public LogEventStackFrame(StackFrame frame)
        {
            if (frame == null)
                return;
            var num = frame.GetFileLineNumber();
            if (num == 0)
                num = frame.GetILOffset();
            var method = frame.GetMethod();
            if (method != null)
            {
                Module = method.DeclaringType != null ? method.DeclaringType.FullName : null;
                Function = method.Name;
                Source = method.ToString();
            }
            else
            {
                Module = "(unknown)";
                Function = "(unknown)";
                Source = "(unknown)";
            }
            Filename = frame.GetFileName();
            LineNumber = num;
            ColumnNumber = frame.GetFileColumnNumber();
            FixNames();
        }

        public string Module { get; set; }

        public string Function { get; set; }

        public string Source { get; set; }

        public string Filename { get; set; }

        public int LineNumber { get; set; }

        public int ColumnNumber { get; set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            if (Module != null)
            {
                stringBuilder.Append(Module);
                stringBuilder.Append('.');
            }
            if (Function != null)
            {
                stringBuilder.Append(Function);
                stringBuilder.Append("()");
            }
            if (Filename != null)
            {
                stringBuilder.Append(" in ");
                stringBuilder.Append(Filename);
            }
            if (LineNumber > -1)
            {
                stringBuilder.Append(":line ");
                stringBuilder.Append(LineNumber);
            }
            return stringBuilder.ToString();
        }

        private void FixNames()
        {
            if (Function == "MoveNext")
            {
                var asyncMatch = asyncRegex.Match(Module);
                if (asyncMatch.Success)
                {
                    Module = asyncMatch.Groups[1].Value;
                    Function = asyncMatch.Groups[2].Value;
                }
            }
            var matchLambda = lambdaRegex.Match(Function);
            if (matchLambda.Success)
            {
                Function = matchLambda.Groups[1].Value + " { <lambda> }";
            }
            if (Module.EndsWith(".<>c") || Module.EndsWith("+<>c"))
                Module = Module.Substring(0, Module.Length - 4);
        }
    }
}