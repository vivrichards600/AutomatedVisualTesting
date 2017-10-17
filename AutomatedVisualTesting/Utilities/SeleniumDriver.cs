using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public static class SeleniumDriver
{
    public enum Browser { Chrome, IE, Firefox };

    /// <summary>
    /// Save screenshot of page loaded from url to Screenshots folder in
    /// project using specified web driver and using page Title as filename
    /// </summary>
    /// <param name="url">Webpage to navigate to</param>
    /// <param name="browser">web browser to use</param>
    public static void SaveScreenShotByUrl(string url, Browser browser = Browser.Chrome)
    {
        IWebDriver driver = null;
        switch (browser)
        {
            case Browser.IE:
                driver = new InternetExplorerDriver();
                break;
            case Browser.Firefox:
                driver = new FirefoxDriver();
                break;
            default:
                driver = new ChromeDriver();
                break;
        }

        driver.Navigate().GoToUrl(url);
        WaitForLoad(driver);
        Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();

        String pageTitle = driver.Title.ToString();
        // TODO: Stick directory in a setting
        String fileDirectory = "../../TestData/";
        if (!Directory.Exists(fileDirectory))
        {
            // screenshot directory doesn't exist
            driver.Quit();
            throw new IOException("Please check screenshots folder exists within test solution to save screenshots");
        }

        String fileName = string.Format("{0}{1}.png", fileDirectory, browser.ToString());
        ss.SaveAsFile(fileName, ImageFormat.Png);

        driver.Quit();
    }

    //public static void SaveElementScreenShotByUrl(string url, IWebElement element, Browser browser = Browser.Chrome)
    public static void SaveElementScreenShotByUrl(string url, string elementSelector, Browser browser = Browser.Chrome)
    {
        IWebDriver driver = null;
        switch (browser)
        {
            case Browser.IE:
                driver = new InternetExplorerDriver();
                break;
            case Browser.Firefox:
                driver = new FirefoxDriver();
                break;
            default:
                driver = new ChromeDriver();
                break;
        }

        driver.Navigate().GoToUrl(url);
        WaitForLoad(driver);

        //find the element! - TODO: Add error catching and magic selector so can specify id, tag or css and it will find first instance? If more than one instance take photos of each!?
        IWebElement element = driver.FindElement(By.CssSelector(elementSelector));

        Byte[] byteArray = ((ITakesScreenshot)driver).GetScreenshot().AsByteArray;
        System.Drawing.Bitmap screenshot = new System.Drawing.Bitmap(new System.IO.MemoryStream(byteArray));
        System.Drawing.Rectangle croppedImage = new System.Drawing.Rectangle(element.Location.X, element.Location.Y, element.Size.Width, element.Size.Height);
        screenshot = screenshot.Clone(croppedImage, screenshot.PixelFormat);

        // TODO: Stick directory in a setting
        String fileDirectory = "../../TestData/";
        if (!Directory.Exists(fileDirectory))
        {
            // screenshot directory doesn't exist
            driver.Quit();
            throw new IOException("Please check screenshots folder exists within test solution to save screenshots");
        }

        String fileName = string.Format("{0}{1}.png", fileDirectory, browser.ToString());

        screenshot.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);

        driver.Quit();
    }



    /// <summary>
    /// Create image of website for the given url
    /// </summary>
    /// <param name="url">Url to take an image of</param>
    /// <returns></returns>
    public static byte[] GetScreenshotByUrl(string url, string elementSelector, Browser browser = Browser.Chrome)
    {
        IWebDriver driver = null;
        switch (browser)
        {
            case Browser.IE:
                driver = new InternetExplorerDriver();
                break;
            case Browser.Firefox:
                driver = new FirefoxDriver();
                break;
            default:
                driver = new ChromeDriver();
                break;
        }
        driver.Navigate().GoToUrl(url.ToString());
        WaitForLoad(driver);

        //find the element! - TODO: Add error catching and magic selector so can specify id, tag or css and it will find first instance? If more than one instance take photos of each!?
        IWebElement element = driver.FindElement(By.CssSelector(elementSelector));

        Byte[] byteArray = ((ITakesScreenshot)driver).GetScreenshot().AsByteArray;
        Bitmap screenshot = new Bitmap(new MemoryStream(byteArray));
        Rectangle croppedImage = new Rectangle(element.Location.X, element.Location.Y, element.Size.Width, element.Size.Height);
        screenshot = screenshot.Clone(croppedImage, screenshot.PixelFormat);


        byte[] bytes = ImageToByte(screenshot);
        driver.Quit();

        return bytes;
    }

    public static byte[] ImageToByte(Image img)
    {
        ImageConverter converter = new ImageConverter();
        return (byte[])converter.ConvertTo(img, typeof(byte[]));
    }


    /// <summary>
    /// Create image of website for the given url
    /// </summary>
    /// <param name="url">Url to take an image of</param>
    /// <returns></returns>
    public static byte[] GetScreenshotByUrl(string url, Browser browser = Browser.Chrome)
    {
        IWebDriver driver = null;
        switch (browser)
        {
            case Browser.IE:
                driver = new InternetExplorerDriver();
                break;
            case Browser.Firefox:
                driver = new FirefoxDriver();
                break;
            default:
                driver = new ChromeDriver();
                break;
        }
        driver.Navigate().GoToUrl(url.ToString());
        WaitForLoad(driver);

        Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
        string screenshot = ss.AsBase64EncodedString;
        byte[] bytes = Convert.FromBase64String(screenshot);

        driver.Quit();

        return bytes;
    }

    /// <summary>
    /// Wait for page to load
    /// </summary>
    /// <param name="driver">web driver</param>
    public static void WaitForLoad(this IWebDriver driver, int timeoutSec = 60)
    {
        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
        WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, timeoutSec));
        wait.Until(wd => js.ExecuteScript("return document.readyState").ToString() == "complete");
    }
}