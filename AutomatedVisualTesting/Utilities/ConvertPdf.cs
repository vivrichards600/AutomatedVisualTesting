using System.Drawing;
using SautinSoft;
using static System.Configuration.ConfigurationManager;

namespace AutomatedVisualTesting.Utilities
{
    public static class ConvertPdf
    {
        public static Image GetPdfPageAsImage(string filename, int page)
        {
            var testDataDirectory = AppSettings.Get("TestDataDirectory");
            var f = new PdfFocus();
            f.OpenPdf(string.Format("{0}{1}", testDataDirectory, filename));
            if (f.PageCount >= page)
            {
                f.ImageOptions.Dpi = 300;
                return f.ToDrawingImage(page);
            }
            return null;
        }

        public static void SavePdfToImage(string filename)
        {
            var testDataDirectory = AppSettings.Get("TestDataDirectory");
            var f = new PdfFocus();
            f.OpenPdf(string.Format("{0}{1}", testDataDirectory, filename));
            if (f.PageCount > 0)
            {
                f.ImageOptions.Dpi = 300;
                for (var p = 1; p < f.PageCount + 1; p = p + 1)
                {
                    var img = f.ToDrawingImage(p);
                    img.Save(string.Format("{0}{1}.{2}.png", testDataDirectory, filename, p));
                }
            }
        }
    }
}