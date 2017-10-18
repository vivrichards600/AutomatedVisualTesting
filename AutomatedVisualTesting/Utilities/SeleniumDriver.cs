using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using static System.Configuration.ConfigurationSettings;

namespace AutomatedVisualTesting.Utilities
{
    public static class SeleniumDriver
    {
        /// <summary>
        ///     Save screenshot of page loaded from url to Screenshots folder in
        ///     project using specified web driver and using page Title as filename
        /// </summary>
        /// <param name="driver">WebDriver</param>
        public static void SaveScreenShotByUrl(IWebDriver driver)
        {
            var testDataDirectory = AppSettings.Get("TestDataDirectory");
            var ss = ((ITakesScreenshot) driver).GetScreenshot();
            if (!Directory.Exists(testDataDirectory))
            {
                // screenshot directory doesn't exist
                driver.Quit();
                throw new IOException("Please check screenshots folder exists within test solution to save screenshots");
            }
            string fileName = $"{testDataDirectory}.png";
            ss.SaveAsFile(fileName, ImageFormat.Png);
            driver.Quit();
        }

        /// <summary>
        ///     Save screenshot of element on page loaded from url to Screenshots folder in
        ///     project using specified web driver and using page Title as filename
        /// </summary>
        /// <param name="driver">ẀebDriver</param>
        /// <param name="elementSelector">Element to take snapshot of</param>
        public static void SaveElementScreenShotByUrl(IWebDriver driver, string elementSelector)
        {
            var testDataDirectory = AppSettings.Get("TestDataDirectory");
            IWebElement element = null;
            try
            {
                // try to find element by ID
                driver.FindElement(By.Id(elementSelector));
                element = driver.FindElement(By.Id(elementSelector));
            }
            catch
            {
                // try to find element by CSS Selector
                element = driver.FindElement(By.CssSelector(elementSelector));
            }

            var byteArray = ((ITakesScreenshot) driver).GetScreenshot().AsByteArray;
            var screenshot = new Bitmap(new MemoryStream(byteArray));
            try
            {
                var croppedImage = new Rectangle(element.Location.X, element.Location.Y, element.Size.Width,
                    element.Size.Height);
                screenshot = screenshot.Clone(croppedImage, screenshot.PixelFormat);

                if (!Directory.Exists(testDataDirectory))
                {
                    // screenshot directory doesn't exist
                    driver.Quit();
                    throw new IOException(
                        "Please check screenshots folder exists within test solution to save screenshots");
                }
                string fileName = $"{testDataDirectory}.png";
                screenshot.Save(fileName, ImageFormat.Png);
                driver.Quit();
            }
            catch
            {
                // could not find element!
                throw new IOException("Could not find element to take a screenshot");
            }
        }

        /// <summary>
        ///     Create image of website for the given url
        /// </summary>
        /// <param name="driver">WebDriver</param>
        /// <returns></returns>
        public static byte[] GetScreenshotByUrl(IWebDriver driver)
        {
            var ss = ((ITakesScreenshot) driver).GetScreenshot();
            var screenshot = ss.AsBase64EncodedString;
            var bytes = Convert.FromBase64String(screenshot);
            driver.Quit();

            return bytes;
        }

        /// <summary>
        ///     Create image of website for the given url
        /// </summary>
        /// <param name="driver">WebDriver</param>
        /// <param name="elementSelector">Selector to find element</param>
        /// <returns></returns>
        public static byte[] GetScreenshotByUrl(IWebDriver driver, string elementSelector)
        {
            IWebElement element = null;
            try
            {
                // try to find element by ID
                driver.FindElement(By.Id(elementSelector));
                element = driver.FindElement(By.Id(elementSelector));
            }
            catch
            {
                // try to find element by CSS Selector
                element = driver.FindElement(By.CssSelector(elementSelector));
            }

            var byteArray = ((ITakesScreenshot) driver).GetScreenshot().AsByteArray;
            var screenshot = new Bitmap(new MemoryStream(byteArray));
            var croppedImage = new Rectangle(element.Location.X, element.Location.Y, element.Size.Width,
                element.Size.Height);
            screenshot = screenshot.Clone(croppedImage, screenshot.PixelFormat);
            var bytes = ImageToByte(screenshot);
            driver.Quit();

            return bytes;
        }

        /// <summary>
        ///     Returns image as byte array
        /// </summary>
        /// <param name="img">Image to return as Byte array</param>
        /// <returns></returns>
        public static byte[] ImageToByte(Image img)
        {
            var converter = new ImageConverter();
            return (byte[]) converter.ConvertTo(img, typeof(byte[]));
        }

        /// <summary>
        ///     Cover the specified dynamic element on the renedered page
        /// </summary>
        /// <param name="driver">WebDriver</param>
        /// <param name="elementSelector">Element Selector</param>
        public static void CoverDynamicElementBySelector(IWebDriver driver, string elementSelector)
        {
            IWebElement element = null;
            try
            {
                // try to find element by ID
                driver.FindElement(By.Id(elementSelector));
                element = driver.FindElement(By.Id(elementSelector));
            }
            catch
            {
                // try to find element by CSS Selector
                element = driver.FindElement(By.CssSelector(elementSelector));
            }

            // Get position of element which we will overlay with a coloured box
            var elementX = element.Location.X; //element from top
            var elementY = element.Location.Y; // element from left
            var elementWidth = element.Size.Width;
            var elementHeight = element.Size.Height;

            // Set styling to place over the top of the dynamic content
            var style =
                string.Format(
                    "'position:absolute;top:{1}px;left:{0}px;width:{2}px;height:{3}px;color:white;background-color:#8b008b;text-align: center;'",
                    elementX, elementY, elementWidth, elementHeight);

            // Set javascript to execute on browser which will cover the dynamic content
            var replaceDynamicContentScript = "var div = document.createElement('div');div.setAttribute('style'," +
                                              style + ");document.body.appendChild(div); ";
            driver.ExecuteJavaScript(replaceDynamicContentScript);
        }
    }
}