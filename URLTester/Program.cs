using _URLTester.Objects;
using _URLTester.Test;
using System;

namespace _URLTester
{
    class Program
    {


        static void Main(string[] args)
        {
            Console.WriteLine("URLTester version 1.1");
            Console.Write(Environment.NewLine);

            if (args.Length == 0)
            {
                PrintHelp();
                return;
            }
            
            var appArgs = ParseArguments(args);

            if (appArgs.help)
            {
                PrintHelp();
                return;
            }
            
            if (string.IsNullOrEmpty(appArgs.domain) || string.IsNullOrEmpty(appArgs.filePath))
            {
                PrintMissingArguments();
                PrintHelp();
                return;
            }

            IURLTest<UrlData> test = null;
            if (appArgs.mutlithreaded)
            {
                test = new ParallelRedirectTest<UrlData>(appArgs.domain, appArgs.filePath, appArgs.outputText);
            }
            else
            {
                test = new RedirectTest<UrlData>(appArgs.domain, appArgs.filePath, appArgs.outputText);
            }


            var testManager = new RedirectTestManager<UrlData>(test);

            Console.WriteLine("Loading File.....");
            Console.Write(Environment.NewLine);

            if (!testManager.LoadFile())
            {
                //if errors then display them
                testManager.OutPutErrorMessages();
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Running.....");
            Console.Write(Environment.NewLine);

            if (!testManager.TestLinks())
            {
                //turning off now -- displaying on screen and saving in csv
                //Console.WriteLine("Errors.....");
                //if errors then display them
                //testManager.OutPutErrorMessages();
            }

            Console.WriteLine("Results.....");
            Console.Write(Environment.NewLine);
            

            testManager.OutPutResults();
            Console.ReadLine();
        }

        /// <summary>
        /// Parses the application args passed into the applicaiton
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Arguments Object</returns>
        private static Arguments ParseArguments(string[] args)
        {
            try
            {
                var appArgs = new Arguments();

                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-f":
                            i++;
                            appArgs.filePath = args[i];
                            break;
                        case "-d":
                            i++;
                            appArgs.domain = args[i];
                            break;
                        case "-o":
                            i++;
                            appArgs.outputText = args[i];
                            break;
                        case "-t":
                            i++;
                            appArgs.mutlithreaded = true;
                            break;
                        case "-h":
                            appArgs.help = true;
                            break;
                    }
                }
                
                return appArgs;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Prints the help man
        /// </summary>
        private static void PrintHelp()
        {
            Console.WriteLine("Usage: URLTester [-f] [-d] [-o] [-h]");
            Console.WriteLine("");
            Console.WriteLine("Options:");
            Console.WriteLine("\t -f \t \t CSV or Json File Path that contains the url list to be tested.");
            Console.WriteLine("\t -d \t \t Hostname Domain eg. https://www.example.com");
            Console.WriteLine("\t -o \t \t Optional output csv file eg. C:\\test\\output.csv");
            Console.WriteLine("\t -t \t \t Runs test as a mutlithread operation.");
            Console.WriteLine("\t -h Help \t Help Manual");
            Console.WriteLine("");
            Console.WriteLine("Sample Arguements");
            Console.WriteLine("\t" + @" -d https://www.example.com -f C:\301test.csv -o C:\output.csv");
            Console.ReadLine();
        }


        /// <summary>
        /// Simple message informing use that some of the arguments are missing.
        /// </summary>
        private static void PrintMissingArguments()
        {
            Console.WriteLine("Missing Arguments -- Please try again.");
            Console.WriteLine("");
        }
    }

}
