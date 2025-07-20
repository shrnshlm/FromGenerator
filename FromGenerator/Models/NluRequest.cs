namespace FromGenerator.Models
{
    public class NluRequest
    {
        public string Message { get; set; } = string.Empty;
        public string[]? CustomIntents { get; set; }
        public string[]? CustomEntities { get; set; }
    }
}
