using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using static System.Int32;
using static System.Configuration.ConfigurationManager;

namespace AutomatedVisualTesting.Utilities
{
    public class UITestBindingBase
    {
        public static IWebDriver Driver;

        private IWebElement _page;

        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        /// public TestContext TestContext { get; set; }
        public TestContext TestContext = InitializeTestContext.TestContext;

        [TestInitialize]
        public void Startup()
        {
            // Load settings from app.config
            var driverWidth = Parse(AppSettings.Get("DriverWidth"));
            var driverHeight = Parse(AppSettings.Get("DriverHeight"));
            var pageLoadTimeout = Parse(AppSettings.Get("PageLoadTimeout"));
            var baseUrl = AppSettings.Get("BaseUrl");

            // add start time for test
            TestContext.Properties.Add("Start", DateTime.Now.ToLongTimeString());

            // Set Chrome options
            var options = new ChromeOptions();
            // Disable warning "Chrome is being controlled by automated test software" 
            // as this can muck up full page screenshots
            options.AddArguments("disable-infobars");

            // Create new Chrome WebDriver and set properties
            Driver = new ChromeDriver(options);
            Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(pageLoadTimeout); //.Add(new TimeSpan(5));
            Driver.Manage().Window.Size = new Size(driverWidth, driverHeight);

            // Navigate to base url
            Driver.Navigate().GoToUrl(baseUrl);

            // Wait until the page has fully loaded
            WaitForPageLoad();
        }

        public void WaitForPageLoad()
        {
            if (_page != null)
            {
                var waitForCurrentPageToStale = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
                waitForCurrentPageToStale.Until(ExpectedConditions.StalenessOf(_page));
            }

            var waitForDocumentReady = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            waitForDocumentReady.Until(
                wdriver =>
                {
                    var javaScriptExecutor = Driver as IJavaScriptExecutor;
                    return
                        (javaScriptExecutor != null) && javaScriptExecutor.ExecuteScript("return document.readyState")
                            .Equals("complete");
                });

            _page = Driver.FindElement(By.TagName("html"));
        }

        [TestCleanup]
        public void Cleanup()
        {
            //log test result if reporting is switched on
            var reportResults = Convert.ToBoolean(AppSettings.Get("ReportResults"));
            if (reportResults)
                Reporting.AddTestResult(TestContext, Driver);

            // ensure we close down the Chrome WebDrivers
            Driver.Quit();

            //remove any custom text context properties
            TestContext.Properties.Remove("Start");
            TestContext.Properties.Remove("TestInformation");
        }
    }
}