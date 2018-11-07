using AutomatedVisualTesting.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using static AutomatedVisualTesting.Utilities.Compare;

namespace AutomatedVisualTesting
{
    [TestClass]
    public class ExampleAccessibilityWebPageTests
    {
        public static IWebDriver Driver;
        private readonly ComparisonOptions _options = new ComparisonOptions();

        [TestCleanup]
        public void Cleanup()
        {
            Driver.Quit();
        }

        [TestMethod]
        public void Full_Page_Flow_Comparison_Test()
        {
            Driver.Navigate().GoToUrl("http://computer-database.herokuapp.com/computers/new");
            var baseImage = "ComputerDatabaseAddComputerPageFlow.Chrome.png";

            // Experimental!
            SeleniumDriver.CreatePageFlow(Driver);

            var result = Compare.Differences(baseImage, Driver);
            Assert.IsTrue(result.Match);
        }

        [TestInitialize]
        public void Startup()
        {
            var chromeOptions = new ChromeOptions();
            // Disable warning "Chrome is being controlled by automated test software" 
            // as this can muck up full page screenshots
            chromeOptions.AddArguments("disable-infobars");

            Driver = new ChromeDriver(chromeOptions);
        }
    }
}