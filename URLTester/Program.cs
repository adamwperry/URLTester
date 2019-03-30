using UrlTester.Objects;
using UrlTester.Test;
using System;

namespace UrlTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("URLTester version 1.1");
            Console.Write(Environment.NewLine);
            
            var appArgs = ParseArguments(args);
            
            if (string.IsNullOrEmpty(appArgs.Domain) || string.IsNullOrEmpty(appArgs.FilePath))
            {
                PrintMissingArguments();
                PrintHelp();
                return;
            }

            var testManager = new RedirectTestManager<UrlData>(
                                appArgs.Mutlithreaded ?
                                    new ParallelRedirectTest<UrlData>(appArgs.Domain, appArgs.FilePath, appArgs.OutputText) :
                                    new RedirectTest<UrlData>(appArgs.Domain, appArgs.FilePath, appArgs.OutputText));

            Console.WriteLine("Loading File.....");
            Console.Write(Environment.NewLine);

            if (!testManager.LoadFile())
            {
                //if errors then display them
                testManager.OutputErrorMessages(WriteErrorMessagesToConsole);
                Console.ReadLine();
                Environment.Exit(1);
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

            testManager.OutputResults();
            Console.ReadLine();
        }


        /// <summary>
        /// Parses the application args passed into the applicaiton
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Arguments Object</returns>
        private static Arguments ParseArguments(string[] args)
        {
            var appArgs = new Arguments();
            var showHelp = false;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-f":
                        appArgs.FilePath = args[i + 1];
                        break;
                    case "-d":
                        appArgs.Domain = args[i + 1];
                        break;
                    case "-o":
                        appArgs.OutputText = args[i + 1];
                        break;
                    case "-t":
                        appArgs.Mutlithreaded = true;
                        break;
                    case "-h":
                    default:
                        showHelp = true;
                        break;
                }
                i++;
            }

            if (args.Length == 0 || showHelp)
                appArgs.Help = true; 

            return appArgs;
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
            Console.WriteLine(@"\t -o \t \t Optional output csv file eg. C:\test\output.csv");
            Console.WriteLine("\t -t \t \t Runs test as a multithread operation.");
            Console.WriteLine("\t -h Help \t Help Manual");
            Console.WriteLine("");
            Console.WriteLine("Sample Arguments");
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

        private static void WriteErrorMessagesToConsole(string[] messages)
        {
            foreach (var message in messages)
            {
                Console.WriteLine(message);
            }
        }
    }

}
