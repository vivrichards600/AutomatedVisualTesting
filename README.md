# Automated Visual Testing

![alt text](https://github.com/vivrichards600/AutomatedVisualTesting/blob/master/AutomatedVisualTesting/TestData/diff.png "Chrome Differences Screenshot")

## About this framework

Lots of tests rely on Selenium WebDriver and similar tools to automate end to end testing. Whilst this ensures elements are displayed 'somewhere' on a page, they do not guarantee that elements are exactly where they should be and looking as expected! 

Using this 'Visual' approach tests can execute a lot quicker and ensure elements are rendered how we expect them to be rathan than the typical approach of trying to find each element to ensure things have loaded on to a page.

It is also necessary sometimes to check contents of files, which can be quite time consuming to open up pdf files to check that the contents are as expected. Using this framework it is also possible to save all pdf pages as images and compare those to previously saved pdf images to check for changes.

## How it works 

### Base images for websites
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

### Base images for files
Once you are happy with the way your pdf pages look you convert each page in to images using just one line of code using the built in helper. 

``` c#
      SavePdfToImage("1.pdf");
```

### Compare Image to an Image taken by visiting a url which is held in memory:

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

You can also specify which browser to use Chrome (used by default if no browser specified), IE or Firefox. 

``` c#
        int difference = GetDifference(image, url); // same as specifying Browser.Chrome
        int difference = GetDifference(image, url, Browser.Chrome);
        int difference = GetDifference(image, url, Browser.IE);
        int difference = GetDifference(image, url, Browser.Firefox);    
```

### Compare Image with another Image:

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

When your tets fail because results were not as expected, the framework will take screenshots of the expected image (regardless where you check via url, pdf page etc), it will also produce an image displaying where the differences were found and place these images in the TestData foler.
