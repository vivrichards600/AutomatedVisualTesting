// Adaption of open source code found here https://github.com/IDeaSCo/ImageComparison
// .net based image comparison utillity which produces pixel matrix based comaprison
// Capable of detecting a single pixel difference between images

// TODO: Currently not efficient, loads the image each time it checks a byte, need to refactor this

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using static System.Configuration.ConfigurationManager;

namespace AutomatedVisualTesting.Utilities
{
    public static class Compare
    {
        public static TestContext Context = InitializeTestContext.TestContext;
        public static int DivFactor = 10;

        /// <summary>
        ///     colormatrix needed to grayscale an image
        /// </summary>
        private static readonly ColorMatrix ColorMatrix = new ColorMatrix(new[]
        {
            new[] {.3f, .3f, .3f, 0, 0},
            new[] {.59f, .59f, .59f, 0, 0},
            new[] {.11f, .11f, .11f, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
        });

        /// <summary>
        ///     Gets the difference between two images as a percentage
        /// </summary>
        /// <param name="img1">The first image</param>
        /// <param name="img2">The image to compare to</param>
        /// <param name="threshold">How big a difference (out of 255) will be ignored - the default is 1.</param>
        /// <returns>The difference between the two images as a percentage</returns>
        public static float Differences(this Image img1, Image img2, byte threshold = 1)
        {
            var differences = img1.GetDifferences(img2);
            var diffPixels = 0;

            foreach (var b in differences)
                if (b > threshold) diffPixels++;
            return diffPixels/256f;
        }

        /// <summary>
        ///     Gets an image which displays the differences between two images
        /// </summary>
        /// <param name="img1">The first image</param>
        /// <param name="img2">The image to compare with</param>
        /// <param name="threshold">How big a difference (out of 255) will be ignored - the default is 1.</param>
        /// <returns>an image which displays the differences between two images</returns>
        public static Bitmap GetDifferenceImage(this Image img1, Image img2, byte threshold = 1)
        {
            //create a 16x16 tiles image with information about how much the two images differ
            var cellsize = 16; //each tile is 16 pixels wide and high
            int width = img1.Width/DivFactor, height = img1.Height/DivFactor;
            var differences = img1.GetDifferences(img2);
            var originalImage = new Bitmap(img1, width*cellsize + 1, height*cellsize + 1);
            var g = Graphics.FromImage(originalImage);

            for (var y = 0; y < differences.GetLength(1); y++)
                for (var x = 0; x < differences.GetLength(0); x++)
                {
                    var cellValue = differences[x, y];
                    if (cellValue > threshold)
                        g.DrawRectangle(Pens.DarkMagenta, x*cellsize, y*cellsize, cellsize, cellsize);
                }
            return originalImage;
        }

        /// <summary>
        ///     Created an image showing the difference between two images
        /// </summary>
        /// <param name="img1">The first image to compare</param>
        /// <param name="img2">The second image to compare</param>
        public static void CreateDifferenceImage(Image img1, Image img2)
        {
            var outputDirectory = AppSettings.Get("OutputDirectory");
            var reportFilename = AppSettings.Get("ReportFilename");
            // Save difference image
            string differencesFilename = $"{DateTime.Now:yyyy-MM-ddTHH-mm-ss}-Differences.png";
            img2.GetDifferenceImage(img1)
                .Resize(img1.Width, img1.Height)
                .Save($"{outputDirectory}{differencesFilename}");

            Debug.WriteLine("-> Unexpected difference(s) found");
            Debug.WriteLine($@"-> Logging differences screenshot to: - file:///{outputDirectory}\{reportFilename}");

            Context.Properties["TestInformation"] +=
                $"<figure>Differences screenshot {differencesFilename}<a href=\"{differencesFilename}\"><img src=\"{differencesFilename}\" alt=\"Differences\" class=\"float-left\"></a></figure>";

            // Save copy of actual image
            string actualImageFilename = $"{DateTime.Now:yyyy-MM-ddTHH-mm-ss}-ActualImage.png";
            img2.Save($"{outputDirectory}{actualImageFilename}");
            Debug.WriteLine(@"-> Logging actual screenshot to: - file:///" + outputDirectory + actualImageFilename);
            Context.Properties["TestInformation"] +=
                $"<figure>Actual screenshot {actualImageFilename}<a href=\"{actualImageFilename}\"><img src=\"{actualImageFilename}\" alt=\"Actual\" class=\"float-left\"></a></figure>";
        }

        /// <summary>
        ///     Finds the differences between two images and returns them in a doublearray
        /// </summary>
        /// <param name="img1">The first image</param>
        /// <param name="img2">The image to compare with</param>
        /// <returns>the differences between the two images as a doublearray</returns>
        public static byte[,] GetDifferences(this Image img1, Image img2)
        {
            int width = img1.Width/DivFactor, height = img1.Height/DivFactor;
            var thisOne = (Bitmap) img1.Resize(width, height).GetGrayScaleVersion();
            var theOtherOne = (Bitmap) img2.Resize(width, height).GetGrayScaleVersion();
            var differences = new byte[width, height];
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    differences[x, y] = (byte) Math.Abs(thisOne.GetPixel(x, y).R - theOtherOne.GetPixel(x, y).R);
            return differences;
        }

        /// <summary>
        ///     Returns an image which has been grayscaled
        /// </summary>
        /// <param name="original">The image to grayscale</param>
        /// <returns>A grayscale version of the image</returns>
        public static Image GetGrayScaleVersion(this Image original)
        {
            //create a blank bitmap the same size as original
            var newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            var g = Graphics.FromImage(newBitmap);

            //create some image attributes
            var attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(ColorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();

            return newBitmap;
        }

        /// <summary>
        ///     Returns a resized image
        /// </summary>
        /// <param name="originalImage">The image to resize</param>
        /// <param name="newWidth">The new width in pixels</param>
        /// <param name="newHeight">The new height in pixels</param>
        /// <returns>A resized version of the original image</returns>
        public static Image Resize(this Image originalImage, int newWidth, int newHeight)
        {
            Image smallVersion = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(smallVersion))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }
            return smallVersion;
        }

        /// <summary>
        ///     Get difference between base image of selected element and screenshot of specified element using the WebDriver
        ///     specified
        /// </summary>
        /// <param name="driver">WebDriver</param>
        /// <param name="imageFileName">Base image file name</param>
        /// <param name="elementSelector">element to compare</param>
        /// <param name="threshold">How big a difference (out of 255) will be ignored - the default is 1.</param>
        /// <returns></returns>
        public static int GetDifference(IWebDriver driver, string imageFileName, string elementSelector,
            byte threshold = 1)
        {
            var currentScreenshot = new MemoryStream(SeleniumDriver.GetScreenshotOfCurrentPage(driver, elementSelector));
            var imageFromUrl = Image.FromStream(currentScreenshot);
            var testDataDirectory = AppSettings.Get("TestDataDirectory");

            // first time we run a test we won't have a base image so create one and alert user in output window
            if (!File.Exists(testDataDirectory + imageFileName))
            {
                imageFromUrl.Save(testDataDirectory + imageFileName);
                Debug.WriteLine(@"-> No base image found for - " + imageFileName);
                Debug.WriteLine(@"-> Base image created - " + imageFileName);
                Context.Properties["TestInformation"] +=
                    $"<figure>No base image for {imageFileName} found. New base image created {imageFileName}<a href=\"{imageFileName}\"><img src=\"{imageFileName}\" alt=\"Differences\" class=\"float-left\"></a></figure>";
            }

            var baseImage = Image.FromFile(testDataDirectory + imageFileName);
            var differencePercentage = baseImage.Differences(imageFromUrl, threshold);
            if ((int) (differencePercentage*100) > 0)
            {
                CreateDifferenceImage(baseImage, imageFromUrl);
                Context.Properties["TestInformation"] +=
                    $"<figure>Expected Image {imageFileName}<a href=\"{imageFileName}\"><img src=\"{imageFileName}\" alt=\"Expcted\" class=\"float-left\"></a></figure>";
            }
            return (int) (differencePercentage*100);
        }

        /// <summary>
        ///     Returns how much pixel difference between the specified image and a screenshot
        ///     of the currently rendered web page which is held in memory
        /// </summary>
        /// <param name="driver">WebDriver</param>
        /// <param name="imageFileName">base image filename.png</param>
        /// <param name="threshold">How big a difference (out of 255) will be ignored - the default is 1.</param>
        /// <returns></returns>
        public static int GetDifference(IWebDriver driver, string imageFileName, byte threshold = 1)
        {
            var currentScreenshot = new MemoryStream(SeleniumDriver.GetScreenshotOfCurrentPage(driver));
            var imageFromUrl = Image.FromStream(currentScreenshot);
            var testDataDirectory = AppSettings.Get("TestDataDirectory");

            // first time we run a test we won't have a base image so create one and alert user in output window
            if (!File.Exists(testDataDirectory + imageFileName))
            {
                imageFromUrl.Save(testDataDirectory + imageFileName);
                Debug.WriteLine(@"-> No base image found for - " + imageFileName);
                Debug.WriteLine(@"-> Base image created - " + imageFileName);
                Context.Properties["TestInformation"] +=
                    $"<figure>No base image for {imageFileName} found. New base image created {imageFileName}<a href=\"{imageFileName}\"><img src=\"{imageFileName}\" alt=\"Differences\" class=\"float-left\"></a></figure>";
            }
            var baseImage = Image.FromFile(testDataDirectory + imageFileName);
            var differencePercentage = baseImage.Differences(imageFromUrl, threshold);
            if ((int) (differencePercentage*100) > 0)
            {
                CreateDifferenceImage(baseImage, imageFromUrl);
                Context.Properties["TestInformation"] +=
                    $"<figure>Expected Image {imageFileName}<a href=\"{imageFileName}\"><img src=\"{imageFileName}\" alt=\"Expcted\" class=\"float-left\"></a></figure>";
            }

            return (int) (differencePercentage*100);
        }

        ///// <summary>
        /////     Get difference between image and a page of a pdf which is converted into
        /////     an image and held in memory to compare
        ///// </summary>
        ///// <param name="baseImage">Base image to compare</param>
        ///// <param name="pdf">pdf file to use</param>
        ///// <param name="page">page of pdf to convert</param>
        ///// <param name="threshold">How big a difference (out of 255) will be ignored - the default is 1.</param>
        ///// <returns>Differences between an image and an image taken from a specified pdf page</returns>
        //public static int GetDifference(string baseImage, string pdf, int page, byte threshold = 1)
        //{
        //    var testDataDirectory = AppSettings.Get("TestDataDirectory");
        //    if (File.Exists(testDataDirectory + baseImage))
        //    {
        //        var img1 = Image.FromFile(testDataDirectory + baseImage);
        //        var img2 = ConvertPdf.GetPdfPageAsImage(pdf, page);
        //        var differencePercentage = img1.Differences(img2, threshold);
        //        if ((int) (differencePercentage*100) > 0)
        //        {
        //            CreateDifferenceImage(img1, img2);
        //            Context.Properties["TestInformation"] +=
        //                $"<figure>Expected Image {baseImage}<a href=\"{baseImage}\"><img src=\"{baseImage}\" alt=\"Expcted\" class=\"float-left\"></a></figure>";
        //            img2.Save($"{testDataDirectory}{pdf}.ImageFromPdf.png");
        //        }
        //        return (int) (differencePercentage*100);
        //    }
        //    return -1;
        //}
    }
}