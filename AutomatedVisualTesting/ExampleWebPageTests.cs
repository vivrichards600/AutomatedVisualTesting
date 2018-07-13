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

        [TestCleanup]
        public void Cleanup()
        {
            Driver.Quit();
        }

        [TestMethod]
        public void Full_Page_Comparison_Covering_Dynamic_Element_Test()
        {
            _options.ShowCellValues = true;

            Driver.Navigate().GoToUrl("https://computer-database.herokuapp.com/computers");
            var baseImage = "ComputerDatabaseHomePageWithoutHeadingAndTable.Chrome.png";

            CoverDynamicElementBySelector(Driver, ".computers");

            _options.Threshold = 20;

            var result = Differences(baseImage, Driver, _options);

            Assert.IsTrue(result.Match);
        }

        [TestMethod]
        public void Full_Page_Comparison_Test()
        {
            // Optionally setting show cell values and passing _options gives you a difference image with tolerance levels drawn!
            _options.ShowCellValues = true;

            Driver.Navigate().GoToUrl("https://computer-database.herokuapp.com/computers/new");
            var baseImage = "ComputerDatabaseAddComputerPage.Chrome.png";
            var result = Differences(baseImage, Driver, _options);

            Assert.IsTrue(result.Match);
        }

        [TestMethod]
        public void Individual_Page_Element_Comparison_Test()
        {
            Driver.Navigate().GoToUrl("https://computer-database.herokuapp.com/computers");
            var element = "add";
            var baseImage = "ComputerDatabaseAddButton.Chrome.png";

            var result = Differences(baseImage, element, Driver);

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