using UrlTester.Objects;

namespace UrlTester.Parsers
{
    public class ArgumentParser
    {
        /// <summary>
        /// Parses the application args passed into the application
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Arguments Object</returns>
        public static Arguments Parse(string[] args)
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

    }
}
