using UrlTester.Objects;
using Core.Objects;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using _URLTester.Output;

namespace UrlTester.Test
{
    /// <summary>
    /// Executes the URL test using a Parallel for each to take advantage of multi-threading
    /// Inherits all other functions from RedirectTest
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ParallelRedirectTest<T>: RedirectTest<T> where T : IUrlData
    {
        public ParallelRedirectTest(string baseURL, string csvString, string outPutFlePath) : base(baseURL, csvString, outPutFlePath)
        {
        }

        public override bool TestLinks()
        {
            ErrorMessages = new List<ErrorMessage>();
            var returnValue = true;

            var i = 1;
            using (var progress = new ConsoleProgressBar(UrlList.Count))
            {
                Parallel.ForEach(UrlList, (item) =>
                {
                    var retval = TestLink(item);
                    if (returnValue && !retval)
                    {
                        returnValue = retval;
                    }

                    progress.Report(i, item.Url);
                    Interlocked.Increment(ref i);
                });
            }

            return returnValue;
        }
    }
}
