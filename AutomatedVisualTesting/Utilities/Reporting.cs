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

            // get todays date ready for report
            var todaysDate = DateTime.Now.ToShortDateString();
            var dateAnchor = todaysDate.Replace("/", "");

            // load html report template
            var reportTemplate =
                File.ReadAllText($@"{testDataDirectory}\ReportTemplate.html")
                    .Replace("<h2>{TodaysDate}</h2>", $"<h2 id=\"{dateAnchor}\">{todaysDate}</h2>")
                    .Replace("<div class=\"col-2\"><h2>Test Runs</h2><ul>",
                        $"<div class=\"col-2\"><h2>Test Runs</h2><ul><li><a href=\"#{dateAnchor}\">{todaysDate}</a></li>");

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
            var testStarted = testContext.Properties["Start"].ToString();
            // set test finish time
            var testFinished = DateTime.Now.ToLongTimeString();
            // get duration of test run
            var duration = DateTime.Parse(testFinished).Subtract(DateTime.Parse(testStarted));

            // directory where html report is stored
            var outputDirectory = AppSettings.Get("OutputDirectory");

            // report file name
            var reportFilename = AppSettings.Get("ReportFilename");

            // check if we have a report file already created
            if (!File.Exists($@"{outputDirectory}\{reportFilename}"))
                CreateReportSkeleton();

            // check test status and style based on result
            var testOutcome = testContext.CurrentTestOutcome.ToString();
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
            var testName = testContext.TestName;
            // get driver type
            var capabilities = ((RemoteWebDriver) driver).Capabilities;
            var browser = capabilities.BrowserName;
            var reportContents = File.ReadAllText($@"{outputDirectory}\{reportFilename}");
            var todaysDate = DateTime.Now.ToShortDateString();
            var dateAnchor = todaysDate.Replace("/", "");
            if (reportContents.Contains(todaysDate))
            {
                //already logged results todays so log results under todays date
                reportContents =
                    reportContents.Replace(
                        $"<h2 id=\"{dateAnchor}\">{todaysDate}</h2><table class=\"table\"><thead><tr><td>Status</td><td>Name</td><td>Browser</td><td>Started</td><td>Finished</td><td>Duration</td></tr></thead><tbody>",
                        $"<h2 id=\"{dateAnchor}\">{todaysDate}</h2><table class=\"table\"><thead><tr><td>Status</td><td>Name</td><td>Browser</td><td>Started</td><td>Finished</td><td>Duration</td></tr></thead><tbody><tr><td>{testOutcome}</td><td>{testName}</td><td class='capitalize'>{browser}</td><td>{testStarted}</td><td>{testFinished}</td><td>{duration}</td><tr>");
                File.WriteAllText($@"{outputDirectory}\{reportFilename}", reportContents);
            }
            else
            {
                // haven't logged results todays, create new heading for todays date and log results
                reportContents = reportContents.Replace("<div class=\"col-2\"><h2>Test Runs</h2><ul>",
                    $"<div class=\"col-2\"><h2>Test Runs</h2><ul><li><a href=\"#{dateAnchor}\">{todaysDate}</a></li>");

                reportContents = reportContents.Replace("<div class=\"col-10\">",
                    $"<div class=\"col-10\"><h2 id=\"{dateAnchor}\">{todaysDate}</h2><table class=\"table\"><thead><tr><td>Status</td><td>Name</td><td>Browser</td><td>Started</td><td>Finished</td><td>Duration</td></tr></thead><tbody><tr><td>{testOutcome}</td><td>{testName}</td><td class='capitalize'>{browser}</td><td>{testStarted}</td><td>{testFinished}</td><td>{duration}</td><tr></tbody></table>");
                File.WriteAllText($@"{outputDirectory}\{reportFilename}", reportContents);
            }
        }
    }
}