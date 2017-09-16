using System.Drawing;

public static class ConvertPDF
{
    /// <summary>
    /// Convert a pdf pages in to images
    /// </summary>
    /// <param name="filename">filename of pdf to save as images</param>
    public static void SavePdfToImage(string filename)
    {
        SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
        f.OpenPdf("../../TestData/" + filename);

        if (f.PageCount > 0)
        {
            f.ImageOptions.Dpi = 300;

            for (int p = 1; p < f.PageCount + 1; p = p + 1)
            {
                Image img = f.ToDrawingImage(p);
                img.Save(string.Format("../../TestData/{0}.{1}.png", filename, p));
            }
        }
    }

    /// <summary>
    /// Convert specified pdf page in to image
    /// </summary>
    /// <param name="filename">filename of pdf</param>
    /// <param name="page">page of pdf</param>
    /// <returns>Pdf page as a png image</returns>
    public static Image GetPdfPageAsImage(string filename, int page)
    {
        SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
        f.OpenPdf("../../TestData/" + filename);

        if (f.PageCount > page)
        {
            f.ImageOptions.Dpi = 300;
            return f.ToDrawingImage(page);
        }
        return null;
    }

}