namespace FromGenerator.Models
{
    public class ClaudeAnalysisResult
    {
        public string Intent { get; set; }
        public double Confidence { get; set; }
        public ClaudeEntity[] Entities { get; set; }
        public string Reasoning { get; set; }
    }

    public class ClaudeEntity
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public double Confidence { get; set; }
    }
}
