using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static ImageTool;

[TestClass]
public class Tests
{
    [TestMethod]
    public void CreateScreenshotByUrlChrome()
    {
        SaveScreenShotByUrl("http://www.google.co.uk/");
    }

    [TestMethod]
    public void CreateScreenshotByUrlIE()
    {
        SaveScreenShotByUrl("http://www.google.co.uk", Browser.IE);
    }

    [TestMethod]
    public void CreateScreenshotByUrlFirefox()
    {
        SaveScreenShotByUrl("http://www.google.co.uk", Browser.Firefox);
    }

    [TestMethod]
    public void CreateImagesFromPdf()
    {
      SavePdfToImage("1.pdf");
    }

    [TestMethod]
    public void WillBeNoDifferenceBetweenImageAndScreenshotByUrlChrome()
    {
        //Arrange
        String image = "Chrome.png";
        Uri url = new Uri("http://www.google.co.uk");

        //Act
        int difference = GetDifference(image, url);

        //Assert
        Assert.IsTrue(difference == 0); // do not allow any difference
    }

    [TestMethod]
    public void WillBeNoDifferenceBetweenImageAndScreenshotByUrlIE()
    {
        //Arrange
        String image = "IE.png";
        Uri url = new Uri("http://www.google.co.uk");

        //Act
        int difference = GetDifference(image, url, Browser.IE);

        //Assert
        Assert.IsTrue(difference == 0); // do not allow any difference
    }

    [TestMethod]
    public void NoDifferenceBetweenImageAndScreenshotByUrlFirefox()
    {
        //Arrange
        String image = "Firefox.png";
        Uri url = new Uri("http://www.google.co.uk");

        //Act
        int difference = GetDifference(image, url, Browser.Firefox);

        //Assert
        Assert.IsTrue(difference == 0); // do not allow any difference
    }


    [TestMethod]
    public void NoDifferenceBetweenPdfImages()
    {
        //Arrange
        String image1 = "1.png";
        String image2 = "1.pdf.1.png";

        //Act
        int difference = GetDifference(image1, image2);

        //Assert
        Assert.IsFalse(difference == 0); // detect difference
    }

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
