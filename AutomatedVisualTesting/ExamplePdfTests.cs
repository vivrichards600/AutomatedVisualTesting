using AutomatedVisualTesting.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static AutomatedVisualTesting.Utilities.ConvertPdf;
using static AutomatedVisualTesting.Utilities.Compare;

namespace AutomatedVisualTesting
{
    [TestClass]
    public class ExamplePdfTests
    {
        private readonly ComparisonOptions _options = new ComparisonOptions();

        [TestMethod]
        public void Pdf_Page_One_Comparison_Test()
        {
            var baseImage = "PDFPage1.png";
            var actualPageImage = GetPdfPageAsImage("Test.pdf", 1);

            var result = Differences(baseImage, actualPageImage, _options);

            Assert.IsTrue(result.Match);
        }

        [TestMethod]
        public void Pdf_Page_Two_Comparison_Test()
        {
            var baseImage = "PDFPage2.png";
            var actualPageImage = GetPdfPageAsImage("Test.pdf", 2);

            var result = Differences(baseImage, actualPageImage, _options);

            Assert.IsTrue(result.Match);
        }
    }
}