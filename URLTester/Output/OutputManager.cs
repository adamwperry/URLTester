using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    public delegate void OutputHandler(string[] messages, string outputPath = null);
}
