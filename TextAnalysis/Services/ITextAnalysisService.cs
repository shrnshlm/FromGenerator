using TextAnalysis.Models;

namespace TextAnalysis.Services
{
    public interface ITextAnalysisService
    {
        Task<AnalysisResponse> AnalyzeTextAsync(AnalysisRequest request);
        Task<bool> IsHealthyAsync();
    }

    public class ClaudeTextAnalysisService : ITextAnalysisService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ClaudeTextAnalysisService> _logger;

        public ClaudeTextAnalysisService(HttpClient httpClient, IConfiguration configuration, ILogger<ClaudeTextAnalysisService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AnalysisResponse> AnalyzeTextAsync(AnalysisRequest request)
        {
            try
            {
                var prompt = $@"
                Analyze this text for form generation purposes:
                Text: '{request.Text}'
                
                Return JSON with intent and entities:
                {{
                  ""intent"": ""BookFlight|HotelReservation|ContactUs|Registration|Feedback|Appointment|Generic"",
                  ""confidence"": 0.85,
                  ""entities"": [
                    {{""type"": ""city"", ""value"": ""Paris"", ""confidence"": 0.90}}
                  ]
                }}";

                // Call Claude API (your existing implementation)
                var claudeResponse = await CallClaudeAPI(prompt);

                return ParseClaudeResponse(claudeResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing text: {Text}", request.Text);
                throw;
            }
        }

        public async Task<bool> IsHealthyAsync()
        {
            try
            {
                var testRequest = new AnalysisRequest { Text = "test", UserId = "health-check" };
                var result = await AnalyzeTextAsync(testRequest);
                return result != null;
            }
            catch
            {
                return false;
            }
        }

        private async Task<string> CallClaudeAPI(string prompt)
        {
            // Your existing Claude API implementation
            return await Task.FromResult("mock response");
        }

        private AnalysisResponse ParseClaudeResponse(string response)
        {
            // Parse Claude response into AnalysisResponse
            return new AnalysisResponse
            {
                Intent = "Generic",
                Confidence = 0.8,
                Entities = new List<ExtractedEntity>(),
                Language = "en",
                ProcessedAt = DateTime.UtcNow
            };
        }
    }
}