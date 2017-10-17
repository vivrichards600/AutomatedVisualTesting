using AutomatedVisualTesting.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomatedVisualTesting
{
    [TestClass]
    public class ExampleTests
    {
        [TestMethod]
        public void NoDifferenceBetweenImageAndScreenshotFromUrl()
        {
            // Arrange
            string baseImage = "ComputerDatabase.png";
            string url = "http://computer-database.herokuapp.com/computers";

            // Act
            int difference = Compare.GetDifference(baseImage, url);

            // Assert
            Assert.IsTrue(difference == 0);
        }

        [TestMethod]
        public void NoDifferenceBetweenElementImageAndScreenshotFromUrl()
        {
            // Arrange
            string baseImage = "Table.png";
            string url = "http://computer-database.herokuapp.com/computers";
            string elementSelector = ".computers";
           // SeleniumDriver.CoverDynamicElementBySelector(".computers");

            // Act
            int difference = Compare.GetDifference(baseImage, url, elementSelector);
            
            // Assert
            Assert.IsTrue(difference == 0);
        }

    }
}
