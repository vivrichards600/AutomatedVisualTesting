using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

[TestClass]
public class Tests
{
    [TestMethod]
    public void CreateInitialScreenshot()
    {
        // Create initial screenshot of website used within regression tests later on
        ImageTool.SaveScreenShotByUrl("http://www.google.com/");
    }

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
}