namespace FromGenerator.Models
{
    public class ClaudeRequest
    {
        public string Model { get; set; } = string.Empty;
        public int Max_tokens { get; set; }
        public ClaudeMessage[] Messages { get; set; } = Array.Empty<ClaudeMessage>();
    }

    public class ClaudeMessage
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

}
