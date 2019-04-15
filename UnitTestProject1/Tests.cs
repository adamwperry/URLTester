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
    private static string _domain;
    private static string _testDirectory;
    private static string _sampleCsvFile;
    private static string _sampleJsonFile;
    private static string _outputFile;


    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        _domain = @"http://localhost:3000";
        _outputFile = @"C:\test\output.csv";
        _testDirectory = $@"{Environment.GetEnvironmentVariable("SystemDrive")}\test";
        _sampleCsvFile = $@"{_testDirectory}\sample.csv";

        var csvData = new string[]
        {
            "URL,expectedRedirect",
            "/verify,http://localhost:3000/test",
            "/help,http://localhost:3000/faq",
            "/,http://localhost:3000/test",
            "/service,http://localhost:3000/service1",
            "/blog,http://localhost:3000/",
            "/contact-us,http://localhost:3000/contact",
            "/email-us,http://localhost:3000/emmail",
        };

        //running into an issue on test during debugging the files still exist.
        ClassCleanup(); 

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

        _sampleJsonFile = $@"{_testDirectory}\sample.json";

        var jsonData = new string[]
        {
            "[",
            "{",
            "\"URL\": \"/verify\",",
            "\"expectedRedirect\": \"http://localhost:3000/test\"",
            "},",
            "{",
            "\"URL\": \"/help\",",
            "\"expectedRedirect\": \"http://localhost:3000/faq\"",
            "},",
            "{",
            "\"URL\": \"/\",",
            "\"expectedRedirect\": \"http://localhost:3000/test\"",
            "},",
            "{",
            "\"URL\": \"/service\",",
            "\"expectedRedirect\": \"http://localhost:3000/service1\"",
            "},",
            "{",
            "\"URL\": \"/blog\",",
            "\"expectedRedirect\": \"http://localhost:3000/\"",
            "},",
            "{",
            "\"URL\": \"/contact-us\",",
            "\"expectedRedirect\": \"http://localhost:3000/contact\"",
            "},",
            "{",
            "\"URL\": \"/email-us\",",
            "\"expectedRedirect\": \"http://localhost:3000/emmail\"",
            "},",
            "]",
        };

        if (!File.Exists(_sampleJsonFile))
        {
            File.Create(_sampleJsonFile).Dispose();
            using (StreamWriter sw = File.CreateText(_sampleJsonFile))
            {
                foreach (var item in jsonData)
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

        if (File.Exists(_sampleJsonFile))
            File.Delete(_sampleJsonFile);
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
        output.AppendLine("\t -o \t \t Optional output csv file eg. C:\\test\\output.csv");
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
            Domain = _domain,
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
            Domain = _domain,
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
    public void Test_RedirectTest_LoadCSVFile_Success()
    {
        var args = new Arguments()
        {
            Domain = _domain,
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
    public void Test_RedirectTest_LoadJsonFile_Success()
    {
        var args = new Arguments()
        {
            Domain = _domain,
            FilePath = _sampleJsonFile,
            OutputText = _outputFile
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
    public void Test_RedirectTest_TestCSVLinks_Fail()
    {
        var args = new Arguments()
        {
            Domain = _domain,
            FilePath = _sampleCsvFile,
            OutputText = _outputFile
        };


        var testManager = new RedirectTestManager<UrlData>(new RedirectTest<UrlData>(args.Domain, args.FilePath, args.OutputText));
        var loadedFile = testManager.LoadFile();

        if (loadedFile)
        {
            Assert.AreEqual(testManager.TestLinks(), false);



            testManager.OutputErrorMessages((string[] messages, string outputPath) =>
            {
                Assert.AreEqual(messages[0], "An error occurred with this url - /verify | The remote server returned an error: (404) Not Found.");
                Assert.AreEqual(messages[1], "An error occurred with this url - /help | The remote server returned an error: (404) Not Found.");
            });
        }
    }

    [TestMethod]
    public void Test_RedirectTest_TestCSVLinks_All()
    {
        var args = new Arguments()
        {
            Domain = _domain,
            FilePath = _sampleCsvFile,
            OutputText = _outputFile
        };


        var testManager = new RedirectTestManager<UrlData>(new RedirectTest<UrlData>(args.Domain, args.FilePath, args.OutputText));
        var loadedFile = testManager.LoadFile();

        if (loadedFile)
        {
            Assert.AreEqual(testManager.TestLinks(), false);

            testManager.OutputResults((string[] messages, string outputPath) =>
            {
                Assert.AreEqual(messages[1], "1, Failed, 0, 0, /verify, http://localhost:3000/test, , The remote server returned an error: (404) Not Found. -- ");
                Assert.AreEqual(messages[2], "2, Failed, 0, 0, /help, http://localhost:3000/faq, , The remote server returned an error: (404) Not Found. -- ");
                Assert.AreEqual(messages[3], "3, Failed, 200, OK, /, http://localhost:3000/test, http://localhost:3000/, \"\"");
                Assert.AreEqual(messages[4], "4, Failed, 200, OK, /service, http://localhost:3000/service1, http://localhost:3000/service, \"\"");
                Assert.AreEqual(messages[5], "5, Passed, 200, OK, /blog, http://localhost:3000/, http://localhost:3000/, \"\"");
                Assert.AreEqual(messages[6], "6, Passed, 200, OK, /contact-us, http://localhost:3000/contact, http://localhost:3000/contact, \"\"");
                Assert.AreEqual(messages[7], "7, Failed, 200, OK, /email-us, http://localhost:3000/emmail, http://localhost:3000/email, \"\"");
            });
        }
    }

    [TestMethod]
    public void Test_RedirectTest_TestJsonLinks_Fail()
    {
        var args = new Arguments()
        {
            Domain = _domain,
            FilePath = _sampleJsonFile,
            OutputText = _outputFile
        };


        var testManager = new RedirectTestManager<UrlData>(new RedirectTest<UrlData>(args.Domain, args.FilePath, args.OutputText));
        var loadedFile = testManager.LoadFile();

        if (loadedFile)
        {
            Assert.AreEqual(testManager.TestLinks(), false);

            testManager.OutputErrorMessages((string[] messages, string outputPath) =>
            {
                Assert.AreEqual(messages[0], "An error occurred with this url - /verify | The remote server returned an error: (404) Not Found.");
                Assert.AreEqual(messages[1], "An error occurred with this url - /help | The remote server returned an error: (404) Not Found.");
            });
        }
    }

    [TestMethod]
    public void Test_RedirectTest_TestJsonLinks_All()
    {
        var args = new Arguments()
        {
            Domain = _domain,
            FilePath = _sampleJsonFile,
            OutputText = _outputFile
        };


        var testManager = new RedirectTestManager<UrlData>(new RedirectTest<UrlData>(args.Domain, args.FilePath, args.OutputText));
        var loadedFile = testManager.LoadFile();

        if (loadedFile)
        {
            Assert.AreEqual(testManager.TestLinks(), false);

            testManager.OutputResults((string[] messages, string outputPath) =>
            {
                Assert.AreEqual(messages[1], "1, Failed, 0, 0, /verify, http://localhost:3000/test, , The remote server returned an error: (404) Not Found. -- ");
                Assert.AreEqual(messages[2], "2, Failed, 0, 0, /help, http://localhost:3000/faq, , The remote server returned an error: (404) Not Found. -- ");
                Assert.AreEqual(messages[3], "3, Failed, 200, OK, /, http://localhost:3000/test, http://localhost:3000/, \"\"");
                Assert.AreEqual(messages[4], "4, Failed, 200, OK, /service, http://localhost:3000/service1, http://localhost:3000/service, \"\"");
                Assert.AreEqual(messages[5], "5, Passed, 200, OK, /blog, http://localhost:3000/, http://localhost:3000/, \"\"");
                Assert.AreEqual(messages[6], "6, Passed, 200, OK, /contact-us, http://localhost:3000/contact, http://localhost:3000/contact, \"\"");
                Assert.AreEqual(messages[7], "7, Failed, 200, OK, /email-us, http://localhost:3000/emmail, http://localhost:3000/email, \"\"");
            });
        }
    }

    [TestMethod]
    public void Test_RedirectTest_OutputResults()
    {
        var args = new Arguments()
        {
            Domain = _domain,
            FilePath = _sampleCsvFile,
            OutputText = _outputFile
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
                Assert.AreEqual(messages[1], "1, Failed, 0, 0, /verify, http://localhost:3000/test, , The remote server returned an error: (404) Not Found. -- ");
                Assert.AreEqual(messages[2], "2, Failed, 0, 0, /help, http://localhost:3000/faq, , The remote server returned an error: (404) Not Found. -- ");
                Assert.AreEqual(messages[3], "3, Failed, 200, OK, /, http://localhost:3000/test, http://localhost:3000/, \"\"");
                Assert.AreEqual(messages[4], "4, Failed, 200, OK, /service, http://localhost:3000/service1, http://localhost:3000/service, \"\"");
                Assert.AreEqual(messages[5], "5, Passed, 200, OK, /blog, http://localhost:3000/, http://localhost:3000/, \"\"");
                Assert.AreEqual(messages[6], "6, Passed, 200, OK, /contact-us, http://localhost:3000/contact, http://localhost:3000/contact, \"\"");
                Assert.AreEqual(messages[7], "7, Failed, 200, OK, /email-us, http://localhost:3000/emmail, http://localhost:3000/email, \"\"");
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

