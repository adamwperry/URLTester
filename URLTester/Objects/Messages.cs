using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLTester.Objects
{
    /// <summary>
    /// Common output messages.
    /// </summary>
    public static class Messages
    {
        public const string ApplicationName = "URLTester";
        public const string ApplicationVersion   = "1.3.1";
        public const string ApplicationTitle = ApplicationName +" " + ApplicationVersion;
        public const string Ellipsis = "...";
        public const string LoadingFile = "Loading File" + Ellipsis;
        public const string Running = "Running" + Ellipsis;
        public const string Results = "Results" + Ellipsis;

    }
}
