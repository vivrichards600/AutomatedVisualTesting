# Automated Visual Testing

![Differences screenshot](https://github.com/vivrichards600/AutomatedVisualTesting/blob/master/AutomatedVisualTesting/TestData/diff.png "Chrome Differences Screenshot")

## About this framework

Lots of tests rely on Selenium WebDriver and similar tools to automate end to end testing. Whilst this ensures elements are displayed 'somewhere' on a page, they do not guarantee that elements are exactly where they should be and looking as expected! 

Using this 'Visual' approach tests can execute a lot quicker and ensure elements are rendered how we expect them to be rathan than the typical approach of trying to find each element to ensure things have loaded on to a page.

It is also necessary sometimes to check contents of files, which can be quite time consuming to open up pdf files to check that the contents are as expected. Using this framework it is also possible to save all pdf pages as images and compare those to previously saved pdf images to check for changes.

## How it works 

### Base images for websites
Once you are happy with how a particular area or how the whole of your web page looks you write tests using this framework. The first time you run your tests the base images will not exist, you can manually take the base images or this framework will automatically take the base images for you and alert you to tell you that this has been done.

At the top of your test class you need to inherit the UITestBindingBase base class. This will take care of the web driver, and various options which you can configure in the app.config.
 
``` c#
	public class YourTestClass : UITestBindingBase
	{
	// Your tests...
	}
```

To manually take a base image of a web page create an instance of the WebDriver, navigate to the URL you want and then pass the WebDriver instance to the helper:

``` c#
 // Create base image of web page by providing the WebDriver
        SaveScreenShotByUrl(driver);
```


![WebPage screenshot](https://github.com/vivrichards600/AutomatedVisualTesting/blob/master/AutomatedVisualTesting/TestData/HomePage.png "Web Page Screenshot")

To manually take a base image of a particular element or area of a web page, create an instance of the WebDriver, navigate to the URL you want and then pass the WebDriver instance to the helper and specify the selector (specify Id or CssSelector):
``` c#
 // Create initial screenshot of website used within regression tests later on
        SaveElementScreenShotByUrl(driver, ".table"); // take base image by using css selector
        SaveElementScreenShotByUrl(driver, "table"); // take base image by using ID selector
```

![WebElement table screenshot](https://github.com/vivrichards600/AutomatedVisualTesting/blob/master/AutomatedVisualTesting/TestData/TableElement.png "Element Screenshot")

### Base images for pdf files
Once you are happy with the way your pdf pages look you convert each page in to images using just one line of code using the built in helper. 

``` c#
      SavePdfToImage("1.pdf");
```

### Compare a base image of a web page to an image of a web page taken by visiting a url:

``` c#
     [TestMethod]
        public void NoDifferenceBetweenImageAndScreenshotFromUrl()
        {
             // Arrange
            var baseImage = "HomePage.png";

            // Act
            var difference = Compare.GetDifference(_driver, baseImage);

            // Assert
            Assert.IsTrue(difference == 0);
        }
```

### Compare a base image of an element on a web page to an image of an element taken by visiting a url:

``` c#

        [TestMethod]
        public void NoDifferenceBetweenElementImageAndScreenshotFromUrl()
        {
           // Arrange
            var baseImage = "TableElement.png";
            var elementByCssSelector = ".computers";

            // Act
            var difference = Compare.GetDifference(_driver, baseImage, elementByCssSelector);

            // Assert
            Assert.IsTrue(difference == 0);
        }
```

### Compare a base image of a web page, whilst covering a dynamic element to an image of a web page taken by visiting a url:

``` c#
     [TestMethod]
        public void NoDifferenceBetweenImageAndScreenshotFromUrl()
        {
            // Arrange
            var baseImage = "HomePageCoveringDynamicElement.png";
            var elementByCssSelector = ".computers";

            // Cover specified dynamic element on page with blanket
            SeleniumDriver.CoverDynamicElementBySelector(_driver, elementByCssSelector);

            // Act
            var difference = Compare.GetDifference(_driver, baseImage);

            // Assert
            Assert.IsTrue(difference == 0);
        }
```

![Web page with dynamic WebElement table covered screenshot](https://github.com/vivrichards600/AutomatedVisualTesting/blob/master/AutomatedVisualTesting/TestData/HomePageCoveringDynamicElement.png "Element Screenshot")

### Compare pdf page to an Image taken previously from a pdf:

``` c#
 [TestMethod]
    public void DifferenceBetweenPdfPageAndImage()
    {
        //Arrange
        String pdf = "1.pdf";
        int page = 1;
        String image = "1.png";

        //Act
       int difference = GetDifference(image, pdf, page);

        //Assert
        Assert.IsFalse(difference == 0); // do not allow any difference
    }
}
```
## Debugging when tests fail

When your tets fail because results were not as expected, the framework will take screenshots of what it actually compared as well as an image displaying where the differences were found. The directory where these images are stored are configurable in the app.config. You can also switch on or off in app.config whether or not to generate a report for your visual test results.

![Visual test report](https://github.com/vivrichards600/AutomatedVisualTesting/blob/master/AutomatedVisualTesting/TestData/ReportScreenshot.png "Report Screenshot")

## Settings
The app.config contains various settings to enable you to specify:


The path to obtain base images from
``` xml
    <add key="TestDataDirectory" value="../../TestData/" />
```

The path to store difference and actual images to
``` xml
    <add key="OutputDirectory" value="C:\Temp\" /> 
```

Whether or not to create a report for visual test results
``` xml
    <add key="ReportResults" value="True"/> 
```

Filename for test results report
``` xml
    <add key="ReportFilename" value="Report.html" />
```

Base url to start testing
``` xml
    <add key="BaseUrl" value="http://computer-database.herokuapp.com/computers" />
```
	
The width to set the web driver window
``` xml
    <add key="DriverWidth" value="1024" /> 
```

The height to set the web driver window
``` xml
    <add key="DriverWidth" value="768" /> 
```

The amount of seconds the driver should wait for the page to load
``` xml
    <add key="PageLoadTimeout" value="5" />
```

## TODOs
There are a number of things I want to add/change over the next few months - please feel free to get involved in the project!

* Unit tests!!
* Tidy up reporting
* Storage options - AWS/ Google / Local - set in config?

   