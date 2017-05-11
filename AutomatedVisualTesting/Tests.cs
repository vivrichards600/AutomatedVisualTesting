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
            // Create initial screenshot of website used within regression tests later on
            ImageComparison.SaveScreenShotByUrl("http://google.co.uk");
        }

        [TestMethod]
        public void TestHomePageLooksAsExpected()
        {
            String expectedScreen = "Google.png";
            String actualScreen = "http://google.co.uk";
            Decimal imageDifference = ImageComparison.GetImageDifference(expectedScreen, actualScreen);

            Assert.IsTrue(imageDifference <= acceptableImageDifference, string.Format("Difference:{0}",imageDifference));
        }

        Decimal acceptableImageDifference = Convert.ToDecimal("0.00");
    }
}
