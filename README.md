# Automated Visual Testing

![alt text](https://github.com/vivrichards600/AutomatedVisualTesting/blob/master/AutomatedVisualTesting/TestData/diff.png "Chrome Differences Screenshot")

## About this framework

Lots of tests rely on Selenium WebDriver and similar tools to automate end to end testing. Whilst this ensures elements are displayed 'somewhere' on a page, they do not guarantee that elements are exactly where they should be and looking as expected! 

Using this 'Visual' approach tests can execute a lot quicker and ensure elements are rendered how we expect them to be rathan than the typical approach of trying to find each element to ensure things have loaded on to a page.

It is also necessary sometimes to check contents of files, which can be quite time consuming to open up pdf files to check that the contents are as expected. Using this framework it is also possible to save all pdf pages as images and compare those to previously saved pdf images to check for changes.

## How it works 

### Base images for websites
Once you are happy with how a particular area or how the whole of your web pages look you write tests using this framework. The firt time you run your tests the base images will not exist, you can manually take the base images or this framework will automatically take the base images for you and alert you to tell you that it has done this.

To manually take a base image of a web page:

``` c#
 // Create initial screenshot of website used within regression tests later on
        SaveScreenShotByUrl("http://www.google.com/");
```

To manually take a base image of a particular element or area or a web page:
``` c#
 // Create initial screenshot of website used within regression tests later on
        SaveElementScreenShotByUrl("http://computer-database.herokuapp.com/computers", ".table"); // take base image by using css selector
        SaveElementScreenShotByUrl("http://computer-database.herokuapp.com/computers", "table"); // take base image by using ID selector
```

You can also specify which browser to use: Chrome (used by default if no browser specified), IE or Firefox. 

``` c#
        SaveScreenShotByUrl("http://computer-database.herokuapp.com/computers"); // same as specifying Browser.Chrome
        SaveScreenShotByUrl("http://computer-database.herokuapp.com/computers", Browser.Chrome);
        SaveScreenShotByUrl("http://computer-database.herokuapp.com/computers", Browser.IE);
        SaveScreenShotByUrl("http://computer-database.herokuapp.com/computers", Browser.Firefox);
```

### Base images for files
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
            string baseImage = "ComputerDatabase.png";
            string url = "http://computer-database.herokuapp.com/computers";

            // Act
            int difference = Compare.GetDifference(baseImage, url);

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
            string baseImage = "Table.png";
            string url = "http://computer-database.herokuapp.com/computers";
            string elementSelector = ".computers";

            // Act
            int difference = Compare.GetDifference(baseImage, url, elementSelector);

            // Assert
            Assert.IsTrue(difference == 0);
        }
```

You can also specify which browser to use Chrome (used by default if no browser specified), IE or Firefox. 

``` c#
        int difference = GetDifference(image, url); // same as specifying Browser.Chrome
        int difference = GetDifference(image, url, Browser.Chrome);
        int difference = GetDifference(image, url, Browser.IE);
        int difference = GetDifference(image, url, Browser.Firefox);    
```

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

When your tets fail because results were not as expected, the framework will take screenshots of the expected image (regardless where you check via url, pdf page etc), it will also produce an image displaying where the differences were found and place these images in the TestData folder.

## Settings
The app.config contains two settings to enable you to specify:


The path to obtain base images from
``` xml
    <add key="TestDataDirectory" value="../../TestData/" />
```


The path to store difference and actual images to
``` xml
    <add key="OutputDirectory" value="C:\Temp\" /> 
```


   