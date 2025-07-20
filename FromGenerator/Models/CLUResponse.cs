namespace FromGenerator.Models
{
    public class CLUResponse
    {
        public string Query { get; set; }
        public string TopIntent { get; set; }
        public double TopIntentConfidence { get; set; }
        public IDictionary<string, object> Entities { get; set; }
        public IDictionary<string, double> AllIntents { get; set; }
    }

}
