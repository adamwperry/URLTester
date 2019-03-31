using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlTester.Parsers;
using UrlTester.Objects;
using UrlTester.Test;
using System.IO;
using System.Text;

[TestClass]
public class Tests
{
    private static string _testDirectory;
    private static string _sampleCsvFile;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        _testDirectory = $@"{Environment.GetEnvironmentVariable("SystemDrive")}\test";
        _sampleCsvFile = $@"{_testDirectory}\sample.csv";

        var csvData = new string[]
        {
            "URL,expectedRedirect",
            "/blog,/new",
            "/contact-us,/contact"
        };


        if (!Directory.Exists(_testDirectory))
            Directory.CreateDirectory(_testDirectory);

        if (!File.Exists(_sampleCsvFile))
        {
            File.Create(_sampleCsvFile).Dispose();
            using (StreamWriter sw = File.CreateText(_sampleCsvFile))
            {
                foreach (var item in csvData)
                {
                    sw.WriteLine(item);
                }

                sw.Close();
            }
        }
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        if (File.Exists(_sampleCsvFile))
            File.Delete(_sampleCsvFile);
    }

    [TestMethod]
    public void Test_ParseArguments_Empty()
    {
        var args = new string[] { "" };
        var expectedArgs = new Arguments();
        expectedArgs.Help = true;

        var parsedArgs = ArgumentParser.Parse(args);

        CompareObjectProperties(parsedArgs, expectedArgs);
    }

    [TestMethod]
    public void Test_ParseArguments_Help()
    {
        var args = new string[] { "-h" };
        var expectedArgs = new Arguments();
        expectedArgs.Help = true;

        var parsedArgs = ArgumentParser.Parse(args);

        CompareObjectProperties(parsedArgs, expectedArgs);


        var output = new StringBuilder();

        output.AppendLine("Usage: URLTester [-f] [-d] [-o] [-h]");
        output.AppendLine("");
        output.AppendLine("Options:");
        output.AppendLine("\t -f \t \t CSV or Json File Path that contains the url list to be tested.");
        output.AppendLine("\t -d \t \t Hostname Domain eg. https://www.example.com");
        output.AppendLine(@"\t -o \t \t Optional output csv file eg. C:\test\output.csv");
        output.AppendLine("\t -t \t \t Runs test as a multithread operation.");
        output.AppendLine("\t -h Help \t Help Manual");
        output.AppendLine("");
        output.AppendLine("Sample Arguments");
        output.AppendLine("\t" + @" -d https://www.example.com -f C:\301test.csv -o C:\output.csv");

        UrlTester.Program.PrintHelp((string[] messages, string outputPath) => {
            Assert.AreEqual(messages[0], output.ToString());
        });

    }

    [TestMethod]
    public void Test_ParseArguments_Typical()
    {
        var args = new string[] {   "-d",
                                    "http://example.com",
                                    "-f",
                                    @"D:\projects\URLTester\URLTester\301test.csv",
                                    "-o",
                                    @"C:\test\output.csv"  };

        var expectedArgs = new Arguments()
        {
            Domain = "http://example.com",
            FilePath = @"D:\projects\URLTester\URLTester\301test.csv",
            OutputText = @"C:\test\output.csv"
        };

        var parsedArgs = ArgumentParser.Parse(args);

        CompareObjectProperties(parsedArgs, expectedArgs);
    }


    [TestMethod]
    public void Test_RedirectTest_LoadFile_FileNotExists()
    {
        var args = new Arguments()
        {
            Domain = "http://example.com",
            FilePath = @"badTextFile",
        };

        var testManager = new RedirectTestManager<UrlData>(new RedirectTest<UrlData>(args.Domain, args.FilePath, args.OutputText));
        var loadedFile = testManager.LoadFile();

        Assert.AreEqual(loadedFile, false);

        testManager.OutputErrorMessages((string[] messages, string outputPath) => 
        {
            Assert.AreEqual(messages[0], $"Specified file path, {args.FilePath}, does not exist.");
        });
        

    }

    [TestMethod]
    public void Test_RedirectTest_LoadFile_UnsupportedFileExtension()
    {
        var args = new Arguments()
        {
            Domain = "http://example.com",
            FilePath = @"D:\projects\URLTester\URLTester\301test.exe",
        };

        try
        {
            File.Create(args.FilePath).Dispose();

            var testManager = new RedirectTestManager<UrlData>(new RedirectTest<UrlData>(args.Domain, args.FilePath, args.OutputText));
            var loadedFile = testManager.LoadFile();

            Assert.AreEqual(loadedFile, false);

            testManager.OutputErrorMessages((string[] messages, string outputPath) =>
            {
                Assert.AreEqual(messages[0], "File Extension is not supported.");
            });
        }
        catch (Exception ex)
        {
            Assert.Fail($"Cannot create necessary test file: {ex.Message} ");
        }
        finally
        {
            if (File.Exists(args.FilePath))
                File.Delete(args.FilePath);
        }
    }

    [TestMethod]
    public void Test_RedirectTest_LoadFile_Success()
    {
        var args = new Arguments()
        {
            Domain = "http://example.com",
            FilePath = _sampleCsvFile,
        };


        var testManager = new RedirectTestManager<UrlData>(new RedirectTest<UrlData>(args.Domain, args.FilePath, args.OutputText));
        var loadedFile = testManager.LoadFile();

        Assert.AreEqual(loadedFile, true);

        testManager.OutputErrorMessages((string[] messages, string outputPath) =>
        {
            Assert.AreEqual(messages.Length, 0);
        });

    }

    [TestMethod]
    public void Test_RedirectTest_TestLinks_Fail()
    {
        var args = new Arguments()
        {
            Domain = "http://example.com",
            FilePath = _sampleCsvFile,
            OutputText = @"C:\test\output.csv"
        };


        var testManager = new RedirectTestManager<UrlData>(new RedirectTest<UrlData>(args.Domain, args.FilePath, args.OutputText));
        var loadedFile = testManager.LoadFile();

        if (loadedFile)
        {
            Assert.AreEqual(testManager.TestLinks(), false);

            testManager.OutputErrorMessages((string[] messages, string outputPath) =>
            {
                Assert.AreEqual(messages[0], "An error occurred with this url - /blog | The remote server returned an error: (404) Not Found.");
                Assert.AreEqual(messages[1], "An error occurred with this url - /contact-us | The remote server returned an error: (404) Not Found.");
            });
        }
    }

    [TestMethod]
    public void Test_RedirectTest_OutputResults()
    {
        var args = new Arguments()
        {
            Domain = "http://example.com",
            FilePath = _sampleCsvFile,
            OutputText = @"C:\test\output.csv"
        };

        var testManager = new RedirectTestManager<UrlData>(new RedirectTest<UrlData>(args.Domain, args.FilePath, args.OutputText));
        var loadedFile = testManager.LoadFile();

        if (loadedFile)
        {
            testManager.TestLinks();
            testManager.OutputResults((string[] messages, string outputPath) =>
            {
                Assert.AreEqual(args.OutputText, outputPath);
                Assert.AreEqual(messages[0], "0, Row Number, Test Result, Response Code, Response, url, expected url, actual url, error");
                Assert.AreEqual(messages[1], "1, Failed, 0, 0, /blog, /new, , The remote server returned an error: (404) Not Found. -- ");
                Assert.AreEqual(messages[2], "2, Failed, 0, 0, /contact-us, /contact, , The remote server returned an error: (404) Not Found. -- ");
            });
        }
    }



    /// <summary>
    /// Loops through all object properties to do a shallow compare
    /// </summary>
    /// <param name="obj1"></param>
    /// <param name="obj2"></param>
    private void CompareObjectProperties(object obj1, object obj2)
    {
        foreach (var prop in obj1.GetType().GetProperties())
        {
            Assert.AreEqual(obj2.GetType().GetProperty(prop.Name).GetValue(obj2), prop.GetValue(obj1));
        }
    }

}

