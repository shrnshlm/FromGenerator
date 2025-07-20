namespace FromGenerator.Models
{

    public class LuisResponse
    {
        public string Query { get; set; }
        public string TopIntent { get; set; }
        public double TopIntentScore { get; set; }
        public Dictionary<string, object> Entities { get; set; }
        public Dictionary<string, object> Intents { get; set; }
    }

}

