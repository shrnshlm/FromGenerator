namespace TextAnalysis.Models
{
    public class AnalysisRequest
    {
        public string Text { get; set; }
        public string UserId { get; set; }
        public string Language { get; set; } = "en";
    }

    public class AnalysisResponse
    {
        public string Intent { get; set; }
        public double Confidence { get; set; }
        public List<ExtractedEntity> Entities { get; set; }
        public string Language { get; set; }
        public DateTime ProcessedAt { get; set; }
    }

    public class ExtractedEntity
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public double Confidence { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
    }
}
