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
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) throw new UriFormatException("Please check url provided is valid");

            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl(url);

            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();

            String pageTitle = driver.Title.ToString();
            // TODO: Stick directory in a setting
            String fileDirectory = "../../Screenshots/";
            if (!Directory.Exists(fileDirectory))
            {
                // screenshot directory doesn't exist
                driver.Close();
                throw new IOException("Please check screenshots folder exists within test solution to save screenshots");
            }

            // save screenshot
            String fileName = string.Format("{0}{1}.png", fileDirectory, pageTitle);
            ss.SaveAsFile(fileName, ImageFormat.Png);

            driver.Close();
        }

        /// <summary>
        /// Compares a stored image against against an image taken at runtime
        /// </summary>
        /// <param name="filename">File path of the first image to compare</param>
        /// <param name="url">Url of website to take snapshot of to compare</param>
        /// <returns>Percentage difference of stored image and image taken during runtime</returns>
        public static decimal GetImageDifference(string filename, string url)
        {
            // TODO: Stick directory in a setting
            String filepath = string.Format("../../Screenshots/{0}",filename);
            if (!File.Exists(filepath)) throw new IOException("Please check image path provided is valid");
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) throw new UriFormatException("Please check url provided is valid");

            MemoryStream currentScreenshot = new MemoryStream(ImageComparison.GetScreenshotByUrl(url));

            Bitmap firstBmp = (Bitmap)Image.FromFile(filepath);
            Bitmap secondBmp = (Bitmap)Image.FromStream(currentScreenshot);

            return Convert.ToDecimal(firstBmp.PercentageDifference(secondBmp, 0) * 100);
        }

        private static byte[] GetScreenshotByUrl(string url)
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl(url);

            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
            string screenshot = ss.AsBase64EncodedString;
            byte[] bytes = Convert.FromBase64String(screenshot);

            driver.Close();

            return bytes;
        }
    }
}
