using System;
using System.Net;

namespace _URLTester.Objects
{
    /// <summary>
    /// abstract url object that is used to contain the data for a test.
    /// </summary>
    public abstract class BaseUrlData
    {
        public string url { get; set; }
        public string expectedRedirect { get; set; }

        public Uri actualRedirect { get; set; }
        public HttpStatusCode headerResponseCode { get; set; }

        public bool testfail { get; set; } = false;

        public string errorMessage { get; set; }

    }

    public class UrlData : BaseUrlData
    {

    }
}
