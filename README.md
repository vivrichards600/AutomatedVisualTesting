# Automated Visual Testing

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
        SaveScreenShotByUrl("http://www.vivrichards.co.uk"); // save as specifying Browser.Chrome
        SaveScreenShotByUrl("http://www.vivrichards.co.uk", Browser.Chrome);
        SaveScreenShotByUrl("http://www.vivrichards.co.uk", Browser.IE);
        SaveScreenShotByUrl("http://www.vivrichards.co.uk", Browser.Firefox);
```

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
        SaveScreenShotByUrl("http://www.vivrichards.co.uk"); // save as specifying Browser.Chrome
        SaveScreenShotByUrl("http://www.vivrichards.co.uk", Browser.Chrome);
        SaveScreenShotByUrl("http://www.vivrichards.co.uk", Browser.IE);
        SaveScreenShotByUrl("http://www.vivrichards.co.uk", Browser.Firefox);
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

## Further info

This is a work in progress and the code is just a quick and dirty way to get some automated visual testing done (it needs a bit of work!). Feel free to fork and improve this solution - I'd love any help with this idea.

## How to use

First we need to take screenshots of all the web pages we want to test are still visually the same. We do this by specifying the url of the page and a screenshot will then get saved to the Screenshots folder:

``` c#
 // Create initial screenshot of website used within regression tests later on
        ImageTool.SaveScreenShotByUrl("http://www.google.com/");
```

As you develop your pages you will want to test to check if they are still displayed the way you expect them to. Next we create a test which references the expected screenshot and then give the url which represents the screenshot: 

``` c#
    [TestMethod]
    public void DetectDifferenceBetweenImageAndUrl()
    {
        //Arrange
        String image = "Google.png";
        Uri url = new Uri("http://www.google.com/");

        //Act
        int difference = ImageTool.GetPercentageDifference(image, url);

        //Assert
        Assert.IsTrue(difference == 0); // do not allow any difference

    }
```


You also have the ability to reference two images already captured and check for differences: 

``` c#
   [TestMethod]
    public void DetectASinglePixelDifferenceBetweenTwoImagesTest()
    {
        //Arrange
        String image1 = "GooglePixel1.png";
        String image2 = "GooglePixel2.png";

        //Act
        int difference = ImageTool.GetPercentageDifference(image1, image2);

        //Assert
        Assert.IsTrue(difference == 1); // find 1 pixel difference
    }
```