using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static ImageComparison;

[TestClass]
public class ExampleTests
{
    [TestMethod]
    public void NoDifferenceBetweenImageTwoImages()
    {
        String firstImage = "Cat.png";
        String secondImage = "Cat.png";

        int difference = GetDifference(firstImage, secondImage);

        Assert.IsTrue(difference == 0);
    }

    [TestMethod]
    public void NoDifferenceBetweenImageAndScreenshotByUrl()
    {
        String baseImage = "Chrome.png";
        string imageByUrl = "http://www.google.co.uk";

        int difference = GetDifference(baseImage, imageByUrl);

        Assert.IsTrue(difference == 0);
    }

    [TestMethod]
    public void NoDifferenceBetweenImageAndScreenshotOfElementByUrl()
    {
        String baseImage = "Chrome.png";
        string imageByUrl = "http://www.google.co.uk";


        int difference = GetDifference(baseImage, imageByUrl);

        Assert.IsTrue(difference == 0);
    }

    [TestMethod]
    public void TakeScreenshotOfElementOnly()
    {
      //  SeleniumDriver.SaveElementScreenShotByUrl("http://computer-database.herokuapp.com/computers", ".computers");


        string baseImage = "Table.png";
        string url = "http://computer-database.herokuapp.com/computers";
        string elementSelector = ".computers";

        int difference = GetDifference(baseImage, url, elementSelector);

        Assert.IsTrue(difference == 0);
    }

}
