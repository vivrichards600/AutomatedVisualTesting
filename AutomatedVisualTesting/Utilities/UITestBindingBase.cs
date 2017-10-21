using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using static System.Int32;
using static System.Configuration.ConfigurationManager;

namespace AutomatedVisualTesting.Utilities
{
    public class UITestBindingBase
    {
        public static IWebDriver Driver;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

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

            // Create new Chrome WebDriver and set properties
            Driver = new ChromeDriver();
            Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(pageLoadTimeout); //.Add(new TimeSpan(5));
            Driver.Manage().Window.Size = new Size(driverWidth, driverHeight);

            // Navigate to base url
            Driver.Navigate().GoToUrl(baseUrl);
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
        }
    }
}