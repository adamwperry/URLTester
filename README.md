# URLTester
A simple C# console 301 redirects tester. This application uses a list of urls from an input (csv or json) file.  The application iterates the provided list of URLs and compiles a list of request responses while testing the returned responses match the expected url from the file. Last the test results are dislayed to the user and saved to a csv file.

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

* * *

### Options:  
*         -f              CSV or Json File Path that contains the url list to be tested.  
*         -d              Hostname Domain eg. https://www.example.com  
*         -o              Optional output text file eg. c:/output.csv  
*         -t              Runs test as a mutlithread operation.
*         -h Help         Help Manual
  
Sample Arguements  
*         -d https://www.example.com -f C:\301test.csv -o c:\output.csv  

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
