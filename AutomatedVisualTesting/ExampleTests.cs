using System;
using System.Drawing;
using AutomatedVisualTesting.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using static System.Int32;
using static System.Configuration.ConfigurationSettings;

namespace AutomatedVisualTesting
{
    [TestClass]
    public class ExampleTests : UITestBindingBase
    {
        [TestMethod]
        public void NoDifferenceBetweenImageAndScreenshotFromUrl()
        {
            // Arrange
            var baseImage = "HomePage.png";

            // Act
            var difference = Compare.GetDifference(Driver, baseImage);

            // Assert
            Assert.IsTrue(difference == 0);
        }

        [TestMethod]
        public void NoDifferenceBetweenElementImageAndScreenshotFromUrl()
        {
            // Arrange
            var baseImage = "TableElement.png";
            var elementByCssSelector = ".computers";

            // Act
            var difference = Compare.GetDifference(Driver, baseImage, elementByCssSelector);

            // Assert
            Assert.IsTrue(difference == 0);
        }

        [TestMethod]
        public void NoDifferenceBetweenImageWithDynamicTableAndScreenshotFromUrl()
        {
            // Arrange
            var baseImage = "HomePageCoveringDynamicElement.png";
            var elementByCssSelector = ".computers";

            // Cover specified dynamic element on page with blanket
            SeleniumDriver.CoverDynamicElementBySelector(Driver, elementByCssSelector);

            // Act
            var difference = Compare.GetDifference(Driver, baseImage);

            // Assert
            Assert.IsTrue(difference == 0);
        }
    }
}