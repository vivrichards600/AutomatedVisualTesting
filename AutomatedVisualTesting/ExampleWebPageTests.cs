using AutomatedVisualTesting.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using static AutomatedVisualTesting.Utilities.Compare;
using static AutomatedVisualTesting.Utilities.SeleniumDriver;

namespace AutomatedVisualTesting
{
    [TestClass]
    public class ExampleWebPageTests
    {
        public static IWebDriver Driver;
        private readonly ComparisonOptions _options = new ComparisonOptions();

        [TestInitialize]
        public void Startup()
        {
            var chromeOptions = new ChromeOptions();
            // Disable warning "Chrome is being controlled by automated test software" 
            // as this can muck up full page screenshots
            chromeOptions.AddArguments("disable-infobars");

            Driver = new ChromeDriver(chromeOptions);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Driver.Quit();
        }

        [TestMethod]
        public void Full_Page_Comparison_Test()
        {
            Driver.Navigate().GoToUrl("https://computer-database.herokuapp.com/computers");
            var baseImage = "ComputerDatabaseHomePage.Chrome.png";
            var result = Differences(baseImage, Driver, _options);

            Assert.IsTrue(result.Match);
        }

        [TestMethod]
        public void Full_Page_Comparison_Covering_Dynamic_Element_Test()
        {
            Driver.Navigate().GoToUrl("https://computer-database.herokuapp.com/computers");
            var baseImage = "ComputerDatabaseHomePageWithoutTable.Chrome.png";

            CoverDynamicElementBySelector(Driver, ".computers");

            var result = Differences(baseImage, Driver, _options);

            Assert.IsTrue(result.Match);
        }

        [TestMethod]
        public void Individual_Page_Element_Comparison_Test()
        {
            Driver.Navigate().GoToUrl("https://computer-database.herokuapp.com/computers");
            var element = ".computers";
            var baseImage = "ComputerDatabaseTable.Chrome.png";

            var result = Differences(baseImage, element, Driver);

            Assert.IsTrue(result.Match);
        }
    }
}