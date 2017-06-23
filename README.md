# Automated Visual Testing



![alt text](https://github.com/vivrichards600/AutomatedVisualTesting/blob/master/AutomatedVisualTesting/Screenshots/Chrome.Differences.png "Chrome Differences Screenshot")

Lots of tests rely on Selenium WebDriver and similar tools to automate end to end testing. Whilst this ensures elements are displayed 'somewhere' on a page, they do not guarantee that elements are exactly where they should be and looking as expected! 

Using this 'Visual' approach tests can execute a lot quicker and ensure elements are rendered how we expect them to be rathan than the typical approach of trying to find each element to ensure things have loaded on to a page.

## How it works 
Once you are happy with the way each of your web pages look you take screenshots of each of the pages using the built in helper. 


``` c#
 // Create initial screenshot of website used within regression tests later on
        SaveScreenShotByUrl("http://www.google.com/");
```

You can also specify which browser to use Chrome (used by default if no browser specified), IE or Firefox. 

``` c#
        SaveScreenShotByUrl("http://www.vivrichards.co.uk"); // same as specifying Browser.Chrome
        SaveScreenShotByUrl("http://www.vivrichards.co.uk", Browser.Chrome);
        SaveScreenShotByUrl("http://www.vivrichards.co.uk", Browser.IE);
        SaveScreenShotByUrl("http://www.vivrichards.co.uk", Browser.Firefox);
```
![alt text](https://github.com/vivrichards600/AutomatedVisualTesting/blob/master/AutomatedVisualTesting/Screenshots/Chrome.png "Chrome Screenshot")

Each time you make changes to your project then you can run automated visual regression tests. 

Compare Image to an Image taken by visiting a url and held in memory:

``` c#
    [TestMethod]
    public void DetectDifferenceBetweenImageAndUrlChrome()
    {
        //Arrange
        String image = "Chrome.png";
        Uri url = new Uri("http://www.vivrichards.co.uk");

        //Act
        int difference = GetDifference(image, url);

        //Assert
        Assert.IsTrue(difference == 0); // do not allow any difference
    }
```

If the above test fails then an image taken from visiting the url is saved into the screenshots folder as well as an image displaying the differences found.

You can also specify which browser to use Chrome (used by default if no browser specified), IE or Firefox. 

``` c#
        int difference = GetDifference(image, url); // same as specifying Browser.Chrome
        int difference = GetDifference(image, url, Browser.Chrome);
        int difference = GetDifference(image, url, Browser.IE);
        int difference = GetDifference(image, url, Browser.Firefox);    
```

Compare Image with another Image:

``` c#
    [TestMethod]
    public void DetectDifferenceBetweenImages()
    {
        //Arrange
        String image1 = "Chrome1.png";
        String image2 = "Chrome2.png";
        
        //Act
        int difference = GetDifference(image1, image2);

        //Assert
        Assert.IsTrue(difference == 0); // do not allow any difference
    }
```
If the above test fails then an image displaying the differences is saved into the screenshots folder.
