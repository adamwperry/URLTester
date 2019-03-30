using System;
using System.Net;

namespace URLTester.Objects
{
    /// <summary>
    /// abstract url object that is used to contain the data for a test.
    /// </summary>
    public abstract class BaseUrlData
    {
        public string Url { get; set; }
        public string ExpectedRedirect { get; set; }

        public Uri ActualRedirect { get; set; }
        public HttpStatusCode HeaderResponseCode { get; set; }

        public bool Testfail { get; set; } = false;

        public string ErrorMessage { get; set; }

    }

    public class UrlData : BaseUrlData
    {

    }
}
