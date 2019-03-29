using System;


namespace _URLTester.Test
{
    public class RedirectTestManager<T>
    {
        private readonly IURLTest<T> redirectTest;

        /// <summary>
        /// Setup using a class of IURLTest
        /// </summary>
        /// <param name="redirectTest">IURLTest<T></param>
        public RedirectTestManager(IURLTest<T> redirectTest)
        {
            this.redirectTest = redirectTest ?? throw new ArgumentNullException("parser");
        }
        
        /// <summary>
        /// Load a test file using IURLTest Class provided
        /// </summary>
        /// <returns></returns>
        public bool LoadFile()
        {
            return redirectTest.LoadFile();
        }

        /// <summary>
        /// Test the urls a test file using IURLTest Class provided
        /// </summary>
        /// <returns></returns>
        public bool TestLinks()
        {
            return redirectTest.TestLinks();
        }

        /// <summary>
        /// Output a test results using IURLTest Class provided
        /// </summary>
        /// <returns></returns>
        public bool OutPutResults()
        {
            return redirectTest.OutPutResults();
        }


        /// <summary>
        /// Output a test error message for the operation using IURLTest Class provided
        /// </summary>
        public void OutPutErrorMessages()
        {
           redirectTest.OutPutErrorMessages();
        }
    }

}
