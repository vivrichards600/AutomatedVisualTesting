using AutomatedVisualTesting.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static AutomatedVisualTesting.Utilities.Compare;
using static AutomatedVisualTesting.Utilities.SeleniumDriver;

namespace AutomatedVisualTesting
{
    [TestClass]
    public class ExampleTests : UITestBindingBase
    {
        [TestMethod]
        public void NoDifferenceBetweenImageAndScreenshotFromPage()
        {
            // Arrange
            var baseImage = "HomePage.png";

            // Act
            var difference = GetDifference(Driver, baseImage);

            // Assert
            Assert.IsTrue(difference == 0);
        }

        [TestMethod]
        public void NoDifferenceBetweenElementImageAndScreenshotFromPage()
        {
            // Arrange
            var baseImage = "TableElement.png";
            var elementByCssSelector = ".computers";

            // Act
            var difference = GetDifference(Driver, baseImage, elementByCssSelector);
            
            // Assert
            Assert.IsTrue(difference == 0);
        }

        [TestMethod]
        public void NoDifferenceBetweenImageWithDynamicTableAndScreenshotFromPage()
        {
            // Arrange
            var baseImage = "HomePageCoveringDynamicElement.png";
            var elementByCssSelector = ".computers";

            // Cover specified dynamic element on page with blanket
            CoverDynamicElementBySelector(Driver, elementByCssSelector);

            // Act
            var difference = GetDifference(Driver, baseImage);

            // Assert
            Assert.IsTrue(difference == 0);
        }
    }
}