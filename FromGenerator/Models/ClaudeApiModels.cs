namespace FromGenerator.Models
{
    public class ClaudeApiResponse
    {
        public ClaudeContent[] Content { get; set; }
    }

    public class ClaudeContent
    {
        public string Text { get; set; }
    }
}
