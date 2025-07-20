namespace FromGenerator.Models
{

    public class ClaudeResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public ClaudeContent[] Content { get; set; } = Array.Empty<ClaudeContent>();
        public string Model { get; set; } = string.Empty;
        public string Stop_reason { get; set; } = string.Empty;
        public ClaudeUsage Usage { get; set; } = new();
    }

    //public class ClaudeContent
    //{
    //    public string Type { get; set; } = string.Empty;
    //    public string Text { get; set; } = string.Empty;
    //}

    public class ClaudeUsage
    {
        public int Input_tokens { get; set; }
        public int Output_tokens { get; set; }
    }
}
