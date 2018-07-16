using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using OpenQA.Selenium;
using static AutomatedVisualTesting.Utilities.SeleniumDriver;
using static System.Configuration.ConfigurationManager;

namespace AutomatedVisualTesting.Utilities
{
    public static class Compare
    {
        private const int CELL_SIZE = 16;

        private static readonly string TestDataDirectory = AppSettings.Get("TestDataDirectory");
        private static readonly string OutputDirectory = AppSettings.Get("OutputDirectory");

        private static readonly ColorMatrix ColorMatrix =
            new ColorMatrix(new[]
                {new[] {.3f, .3f, .3f, 0f, 0f}, new[] {.59f, .59f, .59f, 0f, 0f}, new[] {.11f, .11f, .11f, 0f, 0f}, new[] {0f, 0f, 0f, 1f, 0f}, new[] {0f, 0f, 0f, 0f, 1f}});

        public static ComparisonResult Differences(string img1, Image img2, ComparisonOptions options = null)
        {
            if (!File.Exists(TestDataDirectory + img1)) CreateBaseImage(img1, img2);

            var image1 = Image.FromFile(TestDataDirectory + img1);

            return Differences(image1, img2, options);
        }


        public static ComparisonResult Differences(string baseImage, string element, IWebDriver driver, ComparisonOptions options = null)
        {
            if (!File.Exists(TestDataDirectory + baseImage)) CreateBaseImage(baseImage, element, driver);

            var image1 = Image.FromFile(TestDataDirectory + baseImage);
            var imageFromWebDriver = GetScreenshotOfElement(driver, element);

            return Differences(image1, imageFromWebDriver, options);
        }

        public static ComparisonResult Differences(string img1, IWebDriver driver, ComparisonOptions options = null)
        {
            if (!File.Exists(TestDataDirectory + img1)) CreateBaseImage(img1, driver);

            var image1 = Image.FromFile(TestDataDirectory + img1);
            var imageFromWebDriver = Image.FromStream(new MemoryStream(GetScreenshotOfCurrentPage(driver)));

            return Differences(image1, imageFromWebDriver, options);
        }

        public static ComparisonResult Differences(string img1, string img2, ComparisonOptions options = null)
        {
            var image1 = Image.FromFile(TestDataDirectory + img1);
            var image2 = Image.FromFile(TestDataDirectory + img2);

            return Differences(image1, image2, options);
        }

        public static ComparisonResult Differences(Image img1, Image img2, ComparisonOptions options = null)
        {
            if (options == null) options = new ComparisonOptions();

            var differences = GetDifferenceMatrix(img1, img2, options.Threshold);
            var diffPixels = CountDifferingPixels(differences);
            var differencePercentage = diffPixels/(float) differences.Length;

            var result = new ComparisonResult {Match = diffPixels == 0, DifferencePercentage = differencePercentage};

            if (!result.Match && options.CreateDifferenceImage)
            {
                CreateDifferenceImage(img1, img2, options);
                result.DifferenceImage = GetDifferenceImage(img1, differences, options.ShowCellValues);
            }

            return result;
        }

        private static int CountDifferingPixels(byte[,] differences)
        {
            var diffPixels = 0;

            foreach (var cellValue in differences)
                if (cellValue > 0)
                    diffPixels++;

            return diffPixels;
        }

        private static void CreateDifferenceImage(Image img1, Image img2, ComparisonOptions options)
        {
            var differences = GetDifferenceMatrix(img1, img2, options.Threshold);

            using (var diffImage = GetDifferenceImage(img2, differences, options.ShowCellValues))
            {
                //TODO: need a more meaningful output file name, could be difficult to find newly created image by filename
                diffImage.Save(OutputDirectory + Guid.NewGuid() + ".png");
            }
        }

        private static Bitmap GetDifferenceImage(Image baseImage, byte[,] differences, bool showCellValues)
        {
            var differenceImage = new Bitmap(baseImage);

            using (var g = Graphics.FromImage(differenceImage))
            {
                for (var y = 0; y < differences.GetLength(1); y++)
                    for (var x = 0; x < differences.GetLength(0); x++)
                    {
                        var cellValue = differences[x, y];
                        if (cellValue > 0)
                        {
                            var font = new Font("Arial", 8);
                            var cellText = cellValue.ToString();
                            var textSize = g.MeasureString(cellText, font);

                            var cellRectangle = new Rectangle(x*CELL_SIZE, y*CELL_SIZE, CELL_SIZE, CELL_SIZE);
                            g.DrawRectangle(Pens.DarkMagenta, cellRectangle);
                            g.FillRectangle(new SolidBrush(Color.FromArgb(64, 139, 0, 139)), cellRectangle);
                            if (showCellValues)
                            {
                                g.DrawString(cellText, font, Brushes.Black, x*CELL_SIZE + CELL_SIZE/2 - textSize.Width/2 + 1, y*CELL_SIZE + CELL_SIZE/2 - textSize.Height/2 + 1);
                                g.DrawString(cellText, font, Brushes.White, x*CELL_SIZE + CELL_SIZE/2 - textSize.Width/2, y*CELL_SIZE + CELL_SIZE/2 - textSize.Height/2);
                            }
                        }
                    }
            }

            return differenceImage;
        }

        private static byte[,] GetDifferenceMatrix(Image img1, Image img2, byte threshold)
        {
            var width = img1.Width/CELL_SIZE;
            var height = img1.Height/CELL_SIZE;
            var differences = new byte[width, height];

            using (var bmp1 = (Bitmap) img1.PrepareForComparison(width, height))
            {
                using (var bmp2 = (Bitmap) img2.PrepareForComparison(width, height))
                {
                    for (var y = 0; y < height; y++)
                        for (var x = 0; x < width; x++)
                        {
                            var cellValue1 = bmp1.GetPixel(x, y).R;
                            var cellValue2 = bmp2.GetPixel(x, y).R;
                            var cellDifference = (byte) Math.Abs(cellValue1 - cellValue2);
                            if (cellDifference < threshold)
                                cellDifference = 0;

                            differences[x, y] = cellDifference;
                        }
                }
            }
            return differences;
        }

        private static Image PrepareForComparison(this Image original, int targetWidth, int targetHeight)
        {
            Image smallVersion = new Bitmap(targetWidth, targetHeight);
            using (var g = Graphics.FromImage(smallVersion))
            {
                var attributes = new ImageAttributes();
                attributes.SetColorMatrix(ColorMatrix);
                attributes.SetWrapMode(WrapMode.TileFlipXY);
                var destRect = new Rectangle(0, 0, targetWidth, targetHeight);
                g.DrawImage(original, destRect, 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            }

            return smallVersion;
        }
    }
}