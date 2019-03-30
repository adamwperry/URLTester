using System;
using UrlTester.Objects;

namespace UrlTester.Test
{
    public class RedirectTestManager<T> where T: IUrlData
    {
        private readonly IUrlTest<T> _redirectTest;

        /// <summary>
        /// Setup using a class of IURLTest
        /// </summary>
        /// <param name="redirectTest">IURLTest<T></param>
        public RedirectTestManager(IUrlTest<T> redirectTest)
        {
            _redirectTest = redirectTest ?? throw new ArgumentNullException("parser");
        }
        
        /// <summary>
        /// Load a test file using IURLTest Class provided
        /// </summary>
        /// <returns></returns>
        public bool LoadFile()
        {
            return _redirectTest.LoadFile();
        }

        /// <summary>
        /// Test the urls in a test file using IURLTest Class provided
        /// </summary>
        /// <returns></returns>
        public bool TestLinks()
        {
            return _redirectTest.TestLinks();
        }

        /// <summary>
        /// Output a test results using IURLTest Class provided
        /// </summary>
        /// <returns></returns>
        public bool OutputResults()
        {
            return _redirectTest.OutputResults();
        }

        /// <summary>
        /// Output a test error message for the operation using IURLTest Class provided
        /// </summary>
        public void OutputErrorMessages(OutputHandler handler)
        {
           _redirectTest.OutputErrorMessages(handler);
        }
    }

    public delegate void OutputHandler(string[] messages);

}
