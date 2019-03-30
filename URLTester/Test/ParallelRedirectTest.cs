using URLTester.Objects;
using Core.Objects;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace URLTester.Test
{
    /// <summary>
    /// Executes the URL test using a Parallel foreach to take advantage of multithreading
    /// Inherits all other functions from RedirectTest
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ParallelRedirectTest<T> : RedirectTest<T>
    {
        public ParallelRedirectTest(string baseURL, string csvString, string outPutFlePath) : base(baseURL, csvString, outPutFlePath)
        {
        }

        public override bool TestLinks()
        {
            ErrorMessages = new List<ErrorMessage>();
            var returnValue = true;
            
            Parallel.ForEach(UrlList, (item) =>
            {
                var retval = TestLink(item);
                if(returnValue == true && retval == false)
                {
                    returnValue = retval;
                }
            });

            return returnValue;
        }
    }
}
