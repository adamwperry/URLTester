using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UrlTester.Parsers;
using UrlTester.Objects;
using UrlTester.Test;
using System.IO;

[TestClass]
public class Tests
{
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
            OutputText = @"C:\test\output.csv"
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
            OutputText = @"C:\test\output.csv"
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

