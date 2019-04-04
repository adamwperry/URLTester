using UrlTester.Objects;
using UrlTester.Test;
using System;
using System.IO;
using System.Text;
using UrlTester.Output;

namespace UrlTester
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("URLTester version 1.1");
            Console.Write(Environment.NewLine);

            var appArgs = Parsers.ArgumentParser.Parse(args);
            
            if (string.IsNullOrEmpty(appArgs.Domain) || string.IsNullOrEmpty(appArgs.FilePath) || appArgs.Help)
            {
                if (!appArgs.Help) PrintMissingArguments(OutputManager.WriteMessagesToConsole);
                PrintHelp(OutputManager.WriteMessagesToConsole);
                Console.ReadLine();
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
                testManager.OutputErrorMessages(OutputManager.WriteMessagesToConsole);
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

            testManager.OutputResults(OutputManager.WriteMessagesToConsoleAndFile);
            Console.ReadLine();
        }


        /// <summary>
        /// Prints the help man
        /// </summary>
        public static void PrintHelp(OutputHandler handler)
        {
            var output = new StringBuilder();

            output.AppendLine("Usage: URLTester [-f] [-d] [-o] [-h]");
            output.AppendLine("");
            output.AppendLine("Options:");
            output.AppendLine("\t -f \t \t CSV or Json File Path that contains the url list to be tested.");
            output.AppendLine("\t -d \t \t Hostname Domain eg. https://www.example.com");
            output.AppendLine("\t -o \t \t Optional output csv file eg. C:\test\\output.csv");
            output.AppendLine("\t -t \t \t Runs test as a multithread operation.");
            output.AppendLine("\t -h Help \t Help Manual");
            output.AppendLine("");
            output.AppendLine("Sample Arguments");
            output.AppendLine("\t" + @" -d https://www.example.com -f C:\301test.csv -o C:\output.csv");

            handler(new string[] { output.ToString() });
        }

        /// <summary>
        /// Simple message informing use that some of the arguments are missing.
        /// </summary>
        private static void PrintMissingArguments(OutputHandler handler)
        {
            var output = new StringBuilder();
            output.AppendLine("Missing Arguments -- Please try again.");
            output.AppendLine("");

            handler(new string[] { output.ToString() });
        }

    
    }

}
