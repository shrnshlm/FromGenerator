using System.Text.Json.Serialization;

namespace FromGenerator.Models
{
    public class ClaudeApiResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("content")]
        public ClaudeContent[] Content { get; set; }

        [JsonPropertyName("stop_reason")]
        public string StopReason { get; set; }

        [JsonPropertyName("stop_sequence")]
        public string StopSequence { get; set; }

        [JsonPropertyName("usage")]
        public ClaudeUsage Usage { get; set; }
    }

    public class ClaudeContent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class ClaudeUsage
    {
        [JsonPropertyName("input_tokens")]
        public int InputTokens { get; set; }

        [JsonPropertyName("output_tokens")]
        public int OutputTokens { get; set; }
    }
}
