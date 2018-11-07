using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using static System.Configuration.ConfigurationManager;

namespace AutomatedVisualTesting.Utilities
{
    public static class SeleniumDriver
    {
        private static readonly string TestDataDirectory = AppSettings.Get("TestDataDirectory");

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
            var style = string.Format("'position:absolute;top:{1}px;left:{0}px;width:{2}px;height:{3}px;color:white;background-color:#8b008b;text-align: center;'", elementX,
                elementY, elementWidth, elementHeight);

            // Set javascript to execute on browser which will cover the dynamic content
            var replaceDynamicContentScript = "var div = document.createElement('div');div.setAttribute('style'," + style + ");document.body.appendChild(div); ";

            driver.ExecuteJavaScript(replaceDynamicContentScript);
        }

        public static void CreateBaseImage(string img1, IWebDriver driver)
        {
            var baseImage = new Bitmap(new MemoryStream(GetScreenshotOfCurrentPage(driver)));
            baseImage.Save(TestDataDirectory + img1);
        }

        public static void CreateBaseImage(string img1, string element, IWebDriver driver)
        {
            var baseImage = GetScreenshotOfElement(driver, element);
            baseImage.Save(TestDataDirectory + img1);
        }

        public static void CreateBaseImage(string img1, Image img2)
        {
            img2.Save(TestDataDirectory + img1);
        }

        public static Image GetFullPageScreenshot(IWebDriver driver)
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
            var totalWidth = (int) (long) ((IJavaScriptExecutor) driver).ExecuteScript("return document.body.offsetWidth");
            var totalHeight = (int) (long) ((IJavaScriptExecutor) driver).ExecuteScript("return  document.body.parentNode.scrollHeight");

            // Get the size of the viewport
            var viewportWidth = (int) (long) ((IJavaScriptExecutor) driver).ExecuteScript("return document.body.clientWidth");
            var viewportHeight = (int) (long) ((IJavaScriptExecutor) driver).ExecuteScript("return window.innerHeight");

            var testDataDirectory = AppSettings.Get("TestDataDirectory");
            var fileName = string.Format("{0}test.png", testDataDirectory);

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
                    ((IJavaScriptExecutor) driver).ExecuteScript(string.Format("window.scrollBy({0}, {1})", xDiff, yDiff));
                }
                // Take Screenshot
                var screenshot = driver.TakeScreenshot();
                // Build an Image out of the Screenshot
                var screenshotImage = ScreenshotToImage(screenshot);
                // Calculate the source Rectangle
                var sourceRectangle = new Rectangle(viewportWidth - rectangle.Width, viewportHeight - rectangle.Height, rectangle.Width, rectangle.Height);
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

        public static void GetLayoutOnly(IWebDriver driver)
        {
        }

        public static string GetPageText(IWebDriver driver)
        {
            return driver.FindElement(By.TagName("body")).Text;
        }

        public static byte[] GetScreenshotOfCurrentPage(IWebDriver driver)
        {
            var bytes = ImageToByte(GetFullPageScreenshot(driver));
            return bytes;
        }

        public static Image GetScreenshotOfElement(IWebDriver driver, string elementSelector)
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

            var croppedImage = new Rectangle(element.Location.X, element.Location.Y, element.Size.Width, element.Size.Height);
            screenshot = screenshot.Clone(croppedImage, screenshot.PixelFormat);

            return screenshot;
        }

        public static byte[] ImageToByte(Image img)
        {
            var converter = new ImageConverter();
            return (byte[]) converter.ConvertTo(img, typeof(byte[]));
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

        public static void CreatePageFlow(IWebDriver driver)
        {
            // Inject javascript which draws lines to show the tab flow througha page
            var flowScript = @"var inputs=document.querySelectorAll(""input, select, button, a"");for(I=0;I<inputs.length;I++){var currentInput=inputs[I].getBoundingClientRect(),nextInput=null;I<inputs.length-1?nextInput=inputs[I+1].getBoundingClientRect():(lastInput=inputs[I],nextInput=currentInput);var currentInputLeft=currentInput.left+currentInput.width/2,currentInputTop=currentInput.top+currentInput.height/2,nextInputLeft=nextInput.left+nextInput.width/2,nextInputTop=nextInput.top+nextInput.height/2;0==I&&document.body.insertAdjacentHTML(""afterbegin"",'<svg style=""position:absolute;z-index:5555;height:100%;width:100%;"" ><line x1=""0"" y1=""0"" x2=""'+currentInputLeft+'"" y2=""'+currentInputTop+'"" style=""stroke:rgb(255,0,0);stroke-width:3"" /><circle cx=""'+currentInputLeft+'"" cy=""'+currentInputTop+'"" r=""3"" stroke=""red""/></svg>'),document.body.insertAdjacentHTML(""afterbegin"",'<svg style=""position:absolute;z-index:5555;height:100%;width:100%;"" ><line x1=""'+currentInputLeft+'"" y1=""'+currentInputTop+'"" x2=""'+nextInputLeft+'"" y2=""'+nextInputTop+'"" style=""stroke:rgb(255,0,0);stroke-width:3"" /><circle cx=""'+nextInputLeft+'"" cy=""'+nextInputTop+'"" r=""3"" stroke=""red""/></svg>')}";
            driver.ExecuteJavaScript(flowScript);
        }

    }
}