namespace FromGenerator.Models
{
    public class NluAnalysis
    {
        public string Intent { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public Dictionary<string, double> AllIntents { get; set; } = new();
        public Dictionary<string, string> Entities { get; set; } = new();
        public string Explanation { get; set; } = string.Empty;
    }
}
