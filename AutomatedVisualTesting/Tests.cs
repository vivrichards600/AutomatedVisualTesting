using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace AutomatedVisualTesting
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void CreateInitialScreenshots()
        {
           ImageComparison.SaveScreenShotByUrl("http://www.google.co.uk/");
        }

        [TestMethod]
        public void TestHomePageLooksAsExpected()
        {
            String expectedScreenshot = "../../Screenshots/Google.png";
            MemoryStream currentScreenshot = new MemoryStream(ImageComparison.GetScreenshotByUrl("http://www.google.co.uk/"));

            Decimal pageDifference = Convert.ToDecimal(ImageComparison.CompareImages(expectedScreenshot, currentScreenshot));

                Assert.IsTrue(pageDifference < acceptableDifference, string.Format("Image difference was {0}%", pageDifference));
        }



        /// <summary>
        /// Percentage to allow for differences between images
        /// </summary>
        Decimal acceptableDifference = Convert.ToDecimal("0.5");
    }
}
