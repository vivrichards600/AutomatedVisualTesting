// Image Comparison using https://www.codeproject.com/articles/374386/simple-image-comparison-in-net

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using XnaFan.ImageComparison;

namespace AutomatedVisualTesting
{
    static class ImageComparison
    {
        /// <summary>
        /// Save screenshot of page loaded from url to Screenshots 
        /// folder in project using driver.Title as filename
        /// </summary>
        /// <param name="url">Webpage to navigate to</param>
        public static void SaveScreenShotByUrl(string url)
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl(url);

            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
            String pageTitle = driver.Title.ToString();
            String fileName = string.Format("../../Screenshots/{0}.png", pageTitle);

            ss.SaveAsFile(fileName, ImageFormat.Png); //use any of the built in image formating

            driver.Close();
        }

        /// <summary>
        /// Get a screenshot of a webpage by url
        /// </summary>
        /// <param name="url">Webpage to navigate to</param>
        /// <returns></returns>
        public static byte[] GetScreenshotByUrl(string url)
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl(url);

            //TODO: Currently waiting for elements to load, need to refactor
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(50));

            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
            string screenshot = ss.AsBase64EncodedString;
            byte[] bytes = Convert.FromBase64String(screenshot);

            driver.Close();

            return bytes;
        }

        /// <summary>
        /// Used in Try Catch to save screenshot on test failure
        /// </summary>
        /// <param name="memoryStream">Memory stream of screenshot 
        /// which didn't match what we expected</param>
        public static void TestFailureSaveScreenShot(MemoryStream memoryStream)
        {
            MemoryStream stream = memoryStream;
            String filename = string.Format("../../Screenshots/{0}.TestFailureScreenshot.png", DateTime.Now.ToString("dd-MM-yyy.HHmm"));
            System.IO.File.WriteAllBytes(filename, stream.ToArray());

            String failureInformation = string.Format("Test failed. View screenshot {0} for further information.", filename);
        }

        /// <summary>
        /// Compare two images and returns perenctage of 
        /// how much they differe from each other
        /// </summary>
        /// <param name="imagePath">File path of the first image to compare</param>
        /// <param name="imageStream">File stream of second image to compare</param>
        /// <returns></returns>
        public static string CompareImages(string imagePath, MemoryStream imageStream)
        {
            ////compare the two images
            Bitmap firstBmp = (Bitmap)Image.FromFile(imagePath);
            Bitmap secondBmp = (Bitmap)Image.FromStream(imageStream);
            
            // Get Percentage difference between two images
            return string.Format("{0:0.0}", firstBmp.PercentageDifference(secondBmp, 0) * 100);
        }
    }
}
