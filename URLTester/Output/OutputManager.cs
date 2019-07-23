using _URLTester.Output;
using System;
using System.IO;

namespace UrlTester.Output
{
    public class OutputManager
    {
        public static void WriteMessagesToConsole(string[] messages, string outputPath = null)
        {
            foreach (var message in messages)
            {
                Console.WriteLine(message);
            }
        }

        public static void WriteMessagesToConsoleAndFile(string[] messages, string outputPath)
        {
            try
            {
                using (StreamWriter sw = File.CreateText(outputPath))
                {
                    foreach (var item in messages)
                    {
                        sw.WriteLine(item);
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: Add error handling back
            }

            WriteMessagesToConsole(messages);
        }

        public static void WriteProgressToConsole(int currentIndex, int totalCount, string currentItem = null)
        {
            var bar = new ProgessBar(totalCount);
            bar.UpdateProgressBar(currentIndex, currentItem);
        }
    }

    public delegate void OutputHandler(string[] messages, string outputPath = null);
    public delegate void OutputProgressHandler(int currCount, int totalCount, string currentItem = null);
}
