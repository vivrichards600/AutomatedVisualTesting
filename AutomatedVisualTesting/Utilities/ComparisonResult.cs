using System.Drawing;

namespace AutomatedVisualTesting.Utilities
{
    public class ComparisonResult
    {
        public bool Match { get; set; }
        public float DifferencePercentage { get; set; }
        public Image DifferenceImage { get; set; }
    }
}