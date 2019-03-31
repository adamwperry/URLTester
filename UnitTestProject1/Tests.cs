using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlTester.Parsers;
using UrlTester.Objects;
using UrlTester.Test;
using System.IO;

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

        testManager.OutputErrorMessages((string[] messages) => 
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

            testManager.OutputErrorMessages((string[] messages) =>
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

        testManager.OutputErrorMessages((string[] messages) =>
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

            testManager.OutputErrorMessages((string[] messages) =>
            {
                Assert.AreEqual(messages[0], "An error occurred with this url - /blog | The remote server returned an error: (404) Not Found.");
                Assert.AreEqual(messages[1], "An error occurred with this url - /contact-us | The remote server returned an error: (404) Not Found.");
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

