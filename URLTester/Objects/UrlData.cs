using System;
using System.Net;

namespace UrlTester.Objects
{
    /// <summary>
    /// Interface is used to contain the data for a test.
    /// </summary>
    public abstract class IUrlData
    {
        public string Url { get; set; }
        public string ExpectedRedirect { get; set; }
        public Uri ActualRedirect { get; set; }
        public HttpStatusCode HeaderResponseCode { get; set; }
        public bool Testfail { get; set; } = false;
        public string ErrorMessage { get; set; }
    }

    public class UrlData : IUrlData
    {

    }
}
