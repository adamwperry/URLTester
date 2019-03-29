using _URLTester.Objects;
using Core.Objects;
using Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace _URLTester.Test
{
    public class RedirectTest<T> : IURLTest<T>
    {
        protected List<UrlData> urlList;
        protected List<ErrorMessage> errorMessages;
        protected string baseURL;
        protected string filePath;
        protected string outpuFilePatht;

        //ues this dictionary to determine the correct parser to the load the file.               
        private readonly Dictionary<string, IParser<T>> fileExtensions = new Dictionary<string, IParser<T>>
        {
            {".CSV", new CSVParser<T>() },
            {".JSON", new JSONParser<T>() }

        };
        
        public RedirectTest(string baseURL, string filePath, string outPutFlePath)
        {
            this.baseURL = baseURL;
            this.filePath = filePath;
            this.outpuFilePatht = outPutFlePath;
        }

        /// <summary>
        /// Loads the proviide file (filepPath)
        /// Uses the Json or CSV parser depending on the file extenions
        /// </summary>
        /// <returns></returns>
        public bool LoadFile()
        {
            errorMessages = new List<ErrorMessage>();

            //create the correct parser based on the file extension
            IParser<T> parser = null;
            fileExtensions.TryGetValue(Path.GetExtension(filePath).ToUpper(), out parser);

            if(parser == null)
            {
                //todo... create a lib for the messages.
                errorMessages.Add(new ErrorMessage("File Extension is not supported.", true));
            } else
            {
                var fileParser = new FileParser<T>(parser);
                urlList = fileParser.ParseFile<UrlData>(filePath, ref errorMessages);
            }

            if (errorMessages.Count >= 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Execute the list of url test that have been setup
        /// </summary>
        /// <returns>True if no errors occurred / False if an error has occurred during testing.</returns>
        public virtual bool TestLinks()
        {
            errorMessages = new List<ErrorMessage>();
            var returnValue = true;
            
            foreach (var item in urlList)
            {
                var retval = TestLink(item);
                if (returnValue == true && retval == false)
                {
                    returnValue = retval;
                }
            }
     
            return returnValue;
        }


        /// <summary>
        /// Test the url data provided
        /// </summary>
        /// <param name="item">UrlData </param>
        /// <returns>True for a successful test / False if the test attempt failed.</returns>
        protected bool TestLink(UrlData item)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseURL + item.url);
            string responseBody = String.Empty;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    item.headerResponseCode = response.StatusCode;
                    item.actualRedirect = response.ResponseUri;
                }

                if (item.expectedRedirect != item.actualRedirect.ToString())
                {
                    item.testfail = true;
                }
                return true;
            }
            catch (WebException webEx)
            {
                errorMessages.Add(new ErrorMessage(String.Format("An error occurred with this url - {0} | {1}", item.url, webEx.Message)));
                item.errorMessage = string.Format("{0} -- {1}", webEx.Message, webEx.InnerException);
                item.testfail = true;
                return false;
            }
        }

        public bool OutPutResults()
        {
            //using a dictionary to store the output
            var outPutList = new Dictionary<int, string>()
            {
                {0, "Row Number, Test Result, Response Code, Response, url, expected url, actual url, error" }
            };
            
            //build output dictionary 
            var count = 1;
            foreach (var item in urlList)
            {
                outPutList.Add(count, BuildOutPutMessage(item, count));
                count++;
            }

            //creating the output file
            if (!string.IsNullOrEmpty(outpuFilePatht))
            {
                WriteOutputFile(outPutList);
            }

            //displaying the messages on the screen
            foreach (var item in outPutList)
            {
                Console.WriteLine(item.Value);
            }

            return true;
        }

        /// <summary>
        /// Exports the test results using the outPutText file path provided
        /// </summary>
        /// <param name="outPutList"></param>
        private void WriteOutputFile(Dictionary<int, string> outPutList)
        {
            try
            {
                var newPath = MakeUnique(outpuFilePatht);

                using (StreamWriter sw = File.CreateText(newPath.FullName))
                {
                    foreach (var item in outPutList)
                    {
                        sw.WriteLine(item.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessages.Add(new ErrorMessage(ex.Message, true));
            }
        }
        
        /// <summary>
        /// Build an output message for the user
        /// right now this is being used for both the csv output and the screen output.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <returns>string</returns>
        private string BuildOutPutMessage(BaseUrlData item, int count)
        {
            var output = string.Empty;
            var errorMessage = !string.IsNullOrEmpty(item.errorMessage) ? item.errorMessage : "\"\"";

            output = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", count, !item.testfail ? "Passed" : "Failed", ((int)item.headerResponseCode).ToString(), item.headerResponseCode.ToString(), item.url, item.expectedRedirect, item.actualRedirect, errorMessage);
            return output;
        }

        /// <summary>
        /// Creates a unique file name from a given file path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private FileInfo MakeUnique(string path)
        {
            string dir = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string fileExt = Path.GetExtension(path);

            for (int i = 1; ; ++i)
            {
                if (!File.Exists(path))
                    return new FileInfo(path);

                path = Path.Combine(dir, fileName + " " + i + fileExt);
            }
        }


        /// <summary>
        /// Displays out the the console a list of errors that have occurred during the test execution 
        /// </summary>
        public void OutPutErrorMessages()
        {
            foreach(var error in errorMessages)
            {
                Console.WriteLine(error.message);
            }
        }
    }

}
