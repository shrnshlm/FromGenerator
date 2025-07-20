namespace FromGenerator.Models
{
    public class NluResponse
    {
        public string Query { get; set; } = string.Empty;
        public string TopIntent { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public Dictionary<string, double> AllIntents { get; set; } = new();
        public Dictionary<string, string> Entities { get; set; } = new();
        public string Response { get; set; } = string.Empty;
        public string ProcessedBy { get; set; } = "Claude";
    }

}
