using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using static System.Int32;
using static System.Configuration.ConfigurationSettings;

namespace AutomatedVisualTesting.Utilities
{
    public class UITestBindingBase
    {

        public static IWebDriver Driver;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Startup()
        {
            // add start time for test
            TestContext.Properties.Add("Start", DateTime.Now.ToLongTimeString());

            // Create new Chrome WebDriver
            Driver = new ChromeDriver();

            // Get driver height/width from app.config
            var driverWidth = Parse(AppSettings.Get("DriverWidth"));
            var driverHeight = Parse(AppSettings.Get("DriverHeight"));

            // Set driver height/width of window
            Driver.Manage().Window.Size = new Size(driverWidth, driverHeight);

            // Set page load timeout
            // Get driver height/width from app.config
            var pageLoadTimeout = Parse(AppSettings.Get("PageLoadTimeout"));
            Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(pageLoadTimeout);//.Add(new TimeSpan(5));

            // Get Base URL from app.config
            var baseUrl = AppSettings.Get("BaseUrl");

            // Navigate to base url for testing
            Driver.Navigate().GoToUrl(baseUrl);
        }

        [TestCleanup]
        public void Cleanup()
        {
            //log test result
            Reporting.AddTestResult(TestContext, Driver);

            // ensure we close down the Chrome WebDrivers
            Driver.Quit();
        }
    }
}
