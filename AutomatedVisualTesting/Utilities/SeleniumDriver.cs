using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using static System.Configuration.ConfigurationManager;

namespace AutomatedVisualTesting.Utilities
{
    public static class SeleniumDriver
    {
        /// <summary>
        ///     Save screenshot of currently loaded page
        /// </summary>
        /// <param name="driver">WebDriver</param>
        public static void SaveScreenShotOfCurrentPage(IWebDriver driver)
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
        ///     Save screenshot of element on currently loaded page
        /// </summary>
        /// <param name="driver">WebDriver</param>
        /// <param name="elementSelector">Element to take snapshot of</param>
        public static void SaveElementScreenShotOfCurrentPage(IWebDriver driver, string elementSelector)
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
        ///     Get screenshot of currently loaded page
        /// </summary>
        /// <param name="driver">WebDriver</param>
        /// <returns></returns>
        public static byte[] GetScreenshotOfCurrentPage(IWebDriver driver)
        {
            var bytes = ImageToByte(GetScreenShotOfPage(driver));
            driver.Quit();

            return bytes;
        }

        /// <summary>
        ///     Get screenshot of element for currently loaded page
        /// </summary>
        /// <param name="driver">WebDriver</param>
        /// <param name="elementSelector">Selector to find element</param>
        /// <returns></returns>
        public static byte[] GetScreenshotOfCurrentPage(IWebDriver driver, string elementSelector)
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

        /// <summary>
        ///     Returns a screenshot of the current page
        ///     If set in the config to False will return viewport image, if set True returns full page image
        /// </summary>
        /// <param name="driver">WebDriver instance</param>
        /// <returns></returns>
        public static Image GetScreenShotOfPage(IWebDriver driver)
        {
            // If set to false only take screenshot of whats in view and not the whole page
            var fullPageScreenshot = AppSettings.Get("FullPageScreenshot");
            if (Convert.ToBoolean(fullPageScreenshot) == false)
            {
                // return screenshot of what's visible currently in the viewport
                var screenshot = driver.TakeScreenshot();
                return ScreenshotToImage(screenshot);
            }

            // Get the total size of the page
            var totalWidth =
                (int) (long) ((IJavaScriptExecutor) driver).ExecuteScript("return document.body.offsetWidth");
            var totalHeight =
                (int)
                (long) ((IJavaScriptExecutor) driver).ExecuteScript("return  document.body.parentNode.scrollHeight");

            // Get the size of the viewport
            var viewportWidth =
                (int) (long) ((IJavaScriptExecutor) driver).ExecuteScript("return document.body.clientWidth");
            var viewportHeight = (int) (long) ((IJavaScriptExecutor) driver).ExecuteScript("return window.innerHeight");

            var testDataDirectory = AppSettings.Get("TestDataDirectory");
            string fileName = $"{testDataDirectory}test.png";

            // We only care about taking multiple images together if it doesn't already fit
            if ((totalWidth <= viewportWidth) && (totalHeight <= viewportHeight))
            {
                var screenshot = driver.TakeScreenshot();
                return ScreenshotToImage(screenshot);
            }
            // Split the screen in multiple Rectangles
            var rectangles = new List<Rectangle>();
            // Loop until the totalHeight is reached
            for (var y = 0; y < totalHeight; y += viewportHeight)
            {
                var newHeight = viewportHeight;
                // Fix if the height of the element is too big
                if (y + viewportHeight > totalHeight)
                    newHeight = totalHeight - y;
                // Loop until the totalWidth is reached
                for (var x = 0; x < totalWidth; x += viewportWidth)
                {
                    var newWidth = viewportWidth;
                    // Fix if the Width of the Element is too big
                    if (x + viewportWidth > totalWidth)
                        newWidth = totalWidth - x;
                    // Create and add the Rectangle
                    var currRect = new Rectangle(x, y, newWidth, newHeight);
                    rectangles.Add(currRect);
                }
            }
            // Build the Image
            var stitchedImage = new Bitmap(totalWidth, totalHeight);
            // Get all Screenshots and stitch them together
            var previous = Rectangle.Empty;
            foreach (var rectangle in rectangles)
            {
                // Calculate the scrolling (if needed)
                if (previous != Rectangle.Empty)
                {
                    var xDiff = rectangle.Right - previous.Right;
                    var yDiff = rectangle.Bottom - previous.Bottom;
                    // Scroll
                    ((IJavaScriptExecutor) driver).ExecuteScript($"window.scrollBy({xDiff}, {yDiff})");
                }
                // Take Screenshot
                var screenshot = driver.TakeScreenshot();
                // Build an Image out of the Screenshot
                var screenshotImage = ScreenshotToImage(screenshot);
                // Calculate the source Rectangle
                var sourceRectangle = new Rectangle(viewportWidth - rectangle.Width, viewportHeight - rectangle.Height,
                    rectangle.Width, rectangle.Height);
                // Copy the Image
                using (var graphics = Graphics.FromImage(stitchedImage))
                {
                    graphics.DrawImage(screenshotImage, rectangle, sourceRectangle, GraphicsUnit.Pixel);
                }
                // Set the Previous Rectangle
                previous = rectangle;
            }
            return stitchedImage;
        }

        private static Image ScreenshotToImage(Screenshot screenshot)
        {
            Image screenshotImage;
            using (var memStream = new MemoryStream(screenshot.AsByteArray))
            {
                screenshotImage = Image.FromStream(memStream);
            }
            return screenshotImage;
        }
    }
}