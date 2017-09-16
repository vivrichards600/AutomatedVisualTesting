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
        Uri imageByUrl = new Uri("http://www.google.co.uk");

        int difference = GetDifference(baseImage, imageByUrl);

        Assert.IsTrue(difference == 0);
    }
}
