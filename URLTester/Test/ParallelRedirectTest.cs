using UrlTester.Objects;
using Core.Objects;
 using System.Collections.Generic;
using System.Threading.Tasks;
using UrlTester.Output;

namespace UrlTester.Test
{
    /// <summary>
    /// Executes the URL test using a Parallel foreach to take advantage of multithreading
    /// Inherits all other functions from RedirectTest
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ParallelRedirectTest<T>: RedirectTest<T> where T : IUrlData
    {
        private OutputProgressHandler _handler;
        private int _totalCount;

        public ParallelRedirectTest(string baseURL, string csvString, string outPutFlePath) : base(baseURL, csvString, outPutFlePath)
        {
        }

        public override bool TestLinks(OutputProgressHandler handler)
        {
            _handler = handler;
            _totalCount = UrlList.Count;
            ErrorMessages = new List<ErrorMessage>();
            var returnValue = true;

            var i = 0;
            var countLock = new object();
            Parallel.ForEach(UrlList, (item) =>
            {
                var retval = TestLink(item);
                if(returnValue && !retval)
                {
                    returnValue = retval;
                }

                lock (countLock) { UpdateProgressBar(++i, item.Url); }
            });

            return returnValue;
        }

        private void UpdateProgressBar(int i, string currentItem)
        {
            _handler?.Invoke(i, _totalCount, currentItem);
        }
    }
}
