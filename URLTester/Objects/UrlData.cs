using System;
using System.Net;

namespace UrlTester.Objects
{

    public interface IUrlData
    {
        Uri ActualRedirect { get; set; }
        string ErrorMessage { get; set; }
        string ExpectedRedirect { get; set; }
        HttpStatusCode HeaderResponseCode { get; set; }
        bool Testfail { get; set; }
        string Url { get; set; }
    }

    /// <summary>
    /// Interface is used to contain the data for a test.
    /// </summary>
    public abstract class UrlDataBase : IUrlData
    {
        public string Url { get; set; }
        public string ExpectedRedirect { get; set; }
        public Uri ActualRedirect { get; set; }
        public HttpStatusCode HeaderResponseCode { get; set; }
        public bool Testfail { get; set; } = false;
        public string ErrorMessage { get; set; }
    }

    public class UrlData : UrlDataBase
    {

    }
}
