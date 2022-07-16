# URLTester
A simple C# console client that test url redirections. Http status codes 301, 302, ect. This application uses a list of URLs from an input (csv or json) file.  The application iterates the provided list of URLs and compiles a list of request responses while testing the returned responses match the expected URL from the file. Last the test results are dislayed to the user and/or saved to a csv file.

**e.g.** (csv test output file)

0, Row Number, Test Result, Response Code, Response, url, expected url, actual url, error

1, Failed, 0, 0, /verify, http://localhost:3000/test, , The remote server returned an error: (404) Not Found. -- 

2, Passed, 200, OK, /blog, http://localhost:3000/, http://localhost:3000/

3, Failed, 200, OK, /, http://localhost:3000/test, http://localhost:3000/

* * *

### Sample Test File (csv) 
```
URL,expectedRedirect  
/blog,/new  
/contact-us,/contact  
```

### Sample Domain included Test File (csv) 
```
domain,URL,expectedRedirect  
http://localhost:3000,/blog,/new  
http://localhost:3000,/contact-us,/contact  
```

### Sample Test File (Json) 
```
[
    {
      "URL": "/about",
      "expectedRedirect": "https://example.com/about-us"
    },
    {
      "URL": "/links",
      "expectedRedirect": "https://example.com/sitemap.xml"
    }
]
```

### Sample Domain included Test File (Json) 
```
[
    {
      "domain": "http://localhost:3000",
      "URL": "/about",
      "expectedRedirect": "https://example.com/about-us"
    },
    {
      "domain": "http://localhost:3000",	
      "URL": "/links",
      "expectedRedirect": "https://example.com/sitemap.xml"
    }
]
```

* * *

### Options:  
*         -f              CSV or Json File Path that contains the url list to be tested.  
*         -d              Optional Hostname Domain eg. https://www.example.com - The host name can be included as a domain property in the source data  
*         -o              Optional output text file eg. c:/output.csv  
*         -t              Runs test as a mutlithread operation.
*         -h Help         Help Manual
  
Sample Arguements  
*         -d https://www.example.com -f C:\301test.csv -o c:\output.csv  
*         -f C:\301domainIncludedTest.csv -o c:\output.csv  

* * *
### Unit Test
The solution now has a small set of unit test that can be run. Testing can be easliy setup with a local nodejs server and some 301 redirect rules. 

**e.g.** nodejs - 301 redirect
```
app.get('/contact-us', (req, res, next) => {
    res.redirect(301, '/contact');
});
app.get('/contact', (req, res) => res.send('contact'));
```

### Version Changes
**1.4.0** - Updated the .Net version to 4.8, and updated the Newtonsoft.Json to version 13.0.1. Created a new progress bar type for test to run with out console progress bar.

**1.3.1** - Changed the way the expected and the actual url string comparison is done. Added WebUtility.UrlDecode to handle %20 and other encoded characters.

**1.3.0** - Added the ability to have the domain as part of the source data. This allows testing of data from using different domains. eg. example.com and foo.example.com in the same test. The original -d parameter will still work too.

**1.2.1** - Added a progress bar to the application.
