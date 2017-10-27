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
    [TestClass]
    public class InitializeTestContext
    {
        public static TestContext TestContext { get; private set; }

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            TestContext = context;
        }
    }
    public class UITestBindingBase
    {
        public static IWebDriver Driver;

        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
      ///  public TestContext TestContext { get; set; }

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

            // Create new Chrome WebDriver and set properties
            Driver = new ChromeDriver();
            Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(pageLoadTimeout); //.Add(new TimeSpan(5));
            Driver.Manage().Window.Size = new Size(driverWidth, driverHeight);

            // Navigate to base url
            Driver.Navigate().GoToUrl(baseUrl);
            
            // Wait until the page has fully loaded
            WaitForPageLoad();
        }
        
        IWebElement _page = null;

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
                        (Driver as IJavaScriptExecutor).ExecuteScript("return document.readyState").Equals("complete"));

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
