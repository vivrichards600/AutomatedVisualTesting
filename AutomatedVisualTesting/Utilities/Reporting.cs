using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using static System.Configuration.ConfigurationManager;

namespace AutomatedVisualTesting.Utilities
{
    public static class Reporting
    {
        private static void CreateReportSkeleton()
        {
            // directory where html report template resides
            var testDataDirectory = AppSettings.Get("TestDataDirectory");

            // directory where html report will be stored
            var outputDirectory = AppSettings.Get("OutputDirectory");

            // report file name
            var reportFilename = AppSettings.Get("ReportFilename");

            // load html report template
            var reportTemplate = File.ReadAllText($@"{testDataDirectory}\ReportTemplate.html");

            // check if we have a report file already created
            if (!File.Exists($@"{outputDirectory}\{reportFilename}"))
                using (var sw = File.CreateText($@"{outputDirectory}\{reportFilename}"))
                {
                    sw.WriteLine(reportTemplate);
                    sw.Close();
                }
        }

        public static void AddTestResult(TestContext testContext, IWebDriver driver)
        {
            // set test finish time
            string testStarted = testContext.Properties["Start"].ToString();
            // set test finish time
            string testFinished = DateTime.Now.ToLongTimeString();
            // get duration of test run
            TimeSpan duration = DateTime.Parse(testFinished).Subtract(DateTime.Parse(testStarted));

            // directory where html report is stored
            var outputDirectory = AppSettings.Get("OutputDirectory");

            // report file name
            var reportFilename = AppSettings.Get("ReportFilename");

            // check if we have a report file already created
            if (!File.Exists($@"{outputDirectory}\{reportFilename}"))
                CreateReportSkeleton();

            // check test status and style based on result
            string testOutcome = testContext.CurrentTestOutcome.ToString();
            switch (testOutcome)
            {
                case "Passed":
                    testOutcome = $"<span class='btn btn-success'>{testOutcome}</span>";
                    break;
                case "Failed":
                    testOutcome = $"<span class='btn btn-danger'>{testOutcome}</span>";
                    break;
                default:
                    testOutcome = $"<span class='btn btn-warning'>{testOutcome}</span>";
                    break;
            }

            // get test name
            string testName = testContext.TestName;

            // get driver type
            ICapabilities capabilities = ((RemoteWebDriver)driver).Capabilities;
            string browser = capabilities.BrowserName;

            var text = File.ReadAllText($@"{outputDirectory}\{reportFilename}");
            text = text.Replace("<tbody>",
                $"<tbody><tr><td>{testOutcome}</td><td>{testName}</td><td class='capitalize'>{browser}</td><td>{testStarted}</td><td>{testFinished}</td><td>{duration}</td><tr>");
            File.WriteAllText($@"{outputDirectory}\{reportFilename}", text);
        }
    }
}