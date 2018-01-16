using System.Text;

namespace Vostok.Airlock.Logging
{
    public sealed class LogEventStackFrame
    {
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
    }
}