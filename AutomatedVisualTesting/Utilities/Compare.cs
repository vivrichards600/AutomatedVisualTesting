// Adaption of open source code found here https://github.com/IDeaSCo/ImageComparison
// .net based image comparison utillity which produces pixel matrix based comaprison
// Capable of detecting a single pixel difference between images

// TODO:
// Currently not efficient, loads the image each time it checks a byte, need to refactor this
// Code cleanup/refactor

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace AutomatedVisualTesting.Utilities
{
    public static class Compare
    {
        public static int DivFactor = 10;

        /// <summary>
        /// colormatrix needed to grayscale an image
        /// </summary>
        static readonly ColorMatrix ColorMatrix = new ColorMatrix(new float[][]
        {
            new float[] {.3f, .3f, .3f, 0, 0},
            new float[] {.59f, .59f, .59f, 0, 0},
            new float[] {.11f, .11f, .11f, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
        });

        /// <summary>
        /// Gets the difference between two images as a percentage
        /// </summary>
        /// <param name="img1">The first image</param>
        /// <param name="img2">The image to compare to</param>
        /// <param name="threshold">How big a difference (out of 255) will be ignored - the default is 3.</param>
        /// <returns>The difference between the two images as a percentage</returns>
        public static float Differences(this Image img1, Image img2, byte threshold = 0)
        {
            byte[,] differences = img1.GetDifferences(img2);
            int diffPixels = 0;

            foreach (byte b in differences)
            {
                if (b > threshold) { diffPixels++; }
            }
            return diffPixels / 256f;
        }

        /// <summary>
        /// Gets an image which displays the differences between two images
        /// </summary>
        /// <param name="img1">The first image</param>
        /// <param name="img2">The image to compare with</param>
        /// <returns>an image which displays the differences between two images</returns>
        public static Bitmap GetDifferenceImage(this Image img1, Image img2)
        {
            //create a 16x16 tiles image with information about how much the two images differ
            int cellsize = 16;  //each tile is 16 pixels wide and high
            int width = img1.Width / DivFactor, height = img1.Height / DivFactor;
            byte[,] differences = img1.GetDifferences(img2);
            Bitmap originalImage = new Bitmap(img1, width * cellsize + 1, height * cellsize + 1);
            Graphics g = Graphics.FromImage(originalImage);

            for (int y = 0; y < differences.GetLength(1); y++)
            {
                for (int x = 0; x < differences.GetLength(0); x++)
                {
                    byte cellValue = differences[x, y];
                    if (cellValue > 0)
                    {
                        g.DrawRectangle(Pens.DarkMagenta, x * cellsize, y * cellsize, cellsize, cellsize);
                    }
                }
            }
            return originalImage;
        }

        /// <summary>
        /// Created an image showing the difference between two images
        /// </summary>
        /// <param name="img1">The first image to compare</param>
        /// <param name="img2">The second image to compare</param>
        public static void CreateDifferenceImage(Image img1, Image img2)
        {
            // Save difference image
            string differencesFilename = $"{DateTime.Now:yyyy-MM-ddTHH-mm-ss}-Differences.png";
            img2.GetDifferenceImage(img1).Save(@"C:\Temp\" + differencesFilename);

            Debug.WriteLine(@"-> Unexpected difference(s) found");
            Debug.WriteLine(@"-> Logging differences screenshot to: - file:///C:\Temp\" + differencesFilename);

            // Save copy of actual image
            string actualImageFilename = $"{DateTime.Now:yyyy-MM-ddTHH-mm-ss}-ActualImage.png";
            img2.Save(@"C:\Temp\" + actualImageFilename);
            Debug.WriteLine(@"-> Logging actual screenshot to: - file:///C:\Temp\" + actualImageFilename);
        }

        /// <summary>
        /// Finds the differences between two images and returns them in a doublearray
        /// </summary>
        /// <param name="img1">The first image</param>
        /// <param name="img2">The image to compare with</param>
        /// <returns>the differences between the two images as a doublearray</returns>
        public static byte[,] GetDifferences(this Image img1, Image img2)
        {
            int width = img1.Width / DivFactor, height = img1.Height / DivFactor;
            Bitmap thisOne = (Bitmap)img1.Resize(width, height).GetGrayScaleVersion();
            Bitmap theOtherOne = (Bitmap)img2.Resize(width, height).GetGrayScaleVersion();

            byte[,] differences = new byte[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    differences[x, y] = (byte)Math.Abs(thisOne.GetPixel(x, y).R - theOtherOne.GetPixel(x, y).R);
                }
            }
            return differences;
        }

        /// <summary>
        /// Converts an image to grayscale
        /// </summary>
        /// <param name="original">The image to grayscale</param>
        /// <returns>A grayscale version of the image</returns>
        public static Image GetGrayScaleVersion(this Image original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

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
        /// Resizes an image
        /// </summary>
        /// <param name="originalImage">The image to resize</param>
        /// <param name="newWidth">The new width in pixels</param>
        /// <param name="newHeight">The new height in pixels</param>
        /// <returns>A resized version of the original image</returns>
        public static Image Resize(this Image originalImage, int newWidth, int newHeight)
        {
            Image smallVersion = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(smallVersion))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }
            return smallVersion;
        }

        ///// <summary>
        ///// Gets the difference between two images as a percentage
        ///// </summary>
        ///// <returns>The difference between the two images as a percentage</returns>
        ///// <param name="image1">The first image to compare</param>
        ///// <param name="image2">The second image to compare</param>
        ///// <param name="threshold">How big a difference (out of 255) will be ignored - the default is 0.</param>
        ///// <returns>The difference between the two images as a percentage</returns>
        //public static int GetDifference(string firstImage, string secondImage)
        //{
        //    String fileDirectory = "../../TestData/";
        //    // Ensure both images exist
        //    if (File.Exists(fileDirectory + firstImage) && File.Exists(fileDirectory + secondImage))
        //    {
        //        Image img1 = Image.FromFile(fileDirectory + firstImage);
        //        Image img2 = Image.FromFile(fileDirectory + secondImage);

        //        float differencePercentage = img1.Differences(img2, 0);

        //        if (differencePercentage > 0)
        //        {
        //            // take snapshot of difference
        //            CreateDifferenceImage(img1, img2, "TwoImages.");
        //        }
        //        return (int)(differencePercentage * 100);
        //    }
        //    else return -1;
        //}
        public static int GetDifference(string imageFileName, string url, string elementSelector, SeleniumDriver.Browser browser = SeleniumDriver.Browser.Chrome)
        {
            String fileDirectory = "../../TestData/";
            MemoryStream currentScreenshot = new MemoryStream(SeleniumDriver.GetScreenshotByUrl(url, elementSelector, browser));
            Image imageFromUrl = Image.FromStream(currentScreenshot);

            // first time we run a test we won't have a base image so create one and alert user in output window
            if (!File.Exists(fileDirectory + imageFileName))
            {
                imageFromUrl.Save(fileDirectory + imageFileName);

                Debug.WriteLine(@"-> No base image found for - " + imageFileName);
                Debug.WriteLine(@"-> Base image created - " + imageFileName);

            }

            Image baseImage = Image.FromFile(fileDirectory + imageFileName);

            float differencePercentage = baseImage.Differences(imageFromUrl, 0);
            if (differencePercentage > 0)
            {
                // there was a difference, create a image of what differences there were
                CreateDifferenceImage(baseImage, imageFromUrl);
            }
            return (int)(differencePercentage * 100);
        }

        /// <summary>
        /// Returns how much pixel difference between an image on disk and an image held in memory 
        /// </summary>
        /// <param name="imageFileName">base image filename.png</param>
        /// <param name="url">The website to take a snapshot of</param>
        /// <param name="browser">The web browser to use to navigate to the url</param>
        /// <returns></returns>
        public static int GetDifference(string imageFileName, string url, SeleniumDriver.Browser browser = SeleniumDriver.Browser.Chrome)
        {
            String fileDirectory = "../../TestData/";
            MemoryStream currentScreenshot = new MemoryStream(SeleniumDriver.GetScreenshotByUrl(url, browser));
            Image imageFromUrl = Image.FromStream(currentScreenshot);

            // first time we run a test we won't have a base image so create one and alert user in output window
            if (!File.Exists(fileDirectory + imageFileName))
            {
                imageFromUrl.Save(fileDirectory + imageFileName);

                Debug.WriteLine(@"-> No base image found for - " + imageFileName);
                Debug.WriteLine(@"-> Base image created - " + imageFileName);

            }

            Image baseImage = Image.FromFile(fileDirectory + imageFileName);

            float differencePercentage = baseImage.Differences(imageFromUrl, 0);
            if (differencePercentage > 0)
            {
                // there was a difference, create a image of what differences there were
                CreateDifferenceImage(baseImage, imageFromUrl);
            }
            return (int)(differencePercentage * 100);
        }

        /// <summary>
        /// Get difference between image and a page of a pdf which is converted into 
        /// an image and held in memory to compare
        /// </summary>
        /// <param name="baseImage">Base image to compare</param>
        /// <param name="pdf">pdf file to use</param>
        /// <param name="page">page of pdf to convert</param>
        /// <returns>Differences between an image and an image taken from a specified pdf page</returns>
        public static int GetDifference(string baseImage, string pdf, int page)
        {
            String fileDirectory = "../../TestData/BaseImages";
            if (File.Exists(fileDirectory + baseImage))
            {
                // MemoryStream currentScreenshot = new MemoryStream(GetScreenshotByUrl(url, browser));
                Image img1 = Image.FromFile(fileDirectory + baseImage);
                Image img2 = ConvertPdf.GetPdfPageAsImage(pdf, page);

                float differencePercentage = img1.Differences(img2, 0);
                if (differencePercentage > 0)
                {
                    CreateDifferenceImage(img1, img2);
                    img2.Save($"{fileDirectory}{pdf}.ImageFromPdf.png");
                }

                return (int)(differencePercentage * 100);
            }
            else return -1;
        }
    }
}