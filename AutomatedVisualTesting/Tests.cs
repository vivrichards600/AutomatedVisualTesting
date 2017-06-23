using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static ImageTool;

[TestClass]
public class Tests
{
    [TestMethod]
    public void CreateInitialScreenshotChrome()
    {
        // Create initial screenshot of website used within regression tests later on
        SaveScreenShotByUrl("http://www.vivrichards.co.uk/");
    }
    [TestMethod]
    public void CreateInitialScreenshotIE()
    {
        // Create initial screenshot of website used within regression tests later on
        SaveScreenShotByUrl("http://www.vivrichards.co.uk", Browser.IE);
    }
    [TestMethod]
    public void CreateInitialScreenshotFirefox()
    {
        // Create initial screenshot of website used within regression tests later on
        SaveScreenShotByUrl("http://www.vivrichards.co.uk", Browser.Firefox);
    }

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

    [TestMethod]
    public void DetectDifferenceBetweenImageAndUrlIE()
    {
        //Arrange
        String image = "IE.png";
        Uri url = new Uri("http://www.vivrichards.co.uk");

        //Act
        int difference = GetDifference(image, url, Browser.IE);

        //Assert
        Assert.IsTrue(difference == 0); // do not allow any difference
    }

    [TestMethod]
    public void DetectDifferenceBetweenImageAndUrlFirefox()
    {
        //Arrange
        String image = "Firefox.png";
        Uri url = new Uri("http://www.vivrichards.co.uk");

        //Act
        int difference = GetDifference(image, url, Browser.Firefox);

        //Assert
        Assert.IsTrue(difference == 0); // do not allow any difference
    }
}