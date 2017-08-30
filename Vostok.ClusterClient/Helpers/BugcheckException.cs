using System;

namespace Vostok.Clusterclient.Helpers
{
    internal class BugcheckException : Exception
    {
        public BugcheckException(string message)
            : base(message)
        {
        }
    }
}