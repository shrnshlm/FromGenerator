namespace FromGenerator.Models
{
    public class ClaudeConfig
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ApiUrl { get; set; } = "https://api.anthropic.com/v1/messages";
        public string Model { get; set; } = "claude-3-sonnet-20240229";
        public int MaxTokens { get; set; } = 1000;
    }
}
