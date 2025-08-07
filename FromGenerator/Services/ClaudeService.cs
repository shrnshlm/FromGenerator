using FromGenerator.Configuration;
using FromGenerator.Models;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace FromGenerator.Services
{
    public class ClaudeService : IClaudeService
    {
        private readonly HttpClient _httpClient;
        private readonly ClaudeSettings _settings;
        private readonly ILogger<ClaudeService> _logger;

        public ClaudeService(HttpClient httpClient, IOptions<ClaudeSettings> settings, ILogger<ClaudeService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<string> AnalyzeTextForFormGenerationAsync(string text)
        {
            try
            {
                _logger.LogInformation("Claude API Key configured: {HasKey}", !string.IsNullOrEmpty(_settings.ApiKey));
                _logger.LogInformation("Claude API URL: {ApiUrl}", _settings.ApiUrl);
                
                var prompt = $@"
Analyze this user text and return a JSON response with the detected intent and entities for form generation.

User text: ""{text}""

Please analyze the text and return ONLY a JSON object in this exact format:
{{
  ""intent"": ""one of: BookFlight, HotelReservation, ContactUs, Registration, Feedback, Appointment, or Generic"",
  ""confidence"": 0.85,
  ""entities"": [
    {{
      ""type"": ""entity_type"",
      ""value"": ""extracted_value"",
      ""confidence"": 0.90
    }}
  ],
  ""reasoning"": ""brief explanation of why this intent was chosen""
}}

Supported intents:
- BookFlight: Flight booking requests
- HotelReservation: Hotel/accommodation booking
- ContactUs: Contact forms, questions, support requests
- Registration: Sign up, account creation, newsletter subscription
- Feedback: Reviews, ratings, complaints, suggestions
- Appointment: Scheduling meetings, appointments, bookings
- Generic: Anything else

Entity types to extract:
- departure: departure city/location
- destination: destination city/location  
- city: general city/location
- date: dates, times, durations
- person: person names
- email: email addresses
- phone: phone numbers
- company: company/organization names
- product: product/service names
- number: quantities, counts

Return only the JSON object, no other text.";

                var requestBody = new
                {
                    model = _settings.Model,
                    max_tokens = _settings.MaxTokens,
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, _settings.ApiUrl);
                request.Content = content;
                request.Headers.Add("x-api-key", _settings.ApiKey);
                request.Headers.Add("anthropic-version", "2023-06-01");

                var response = await _httpClient.SendAsync(request);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Claude API request failed with status {StatusCode}: {ErrorContent}", 
                        response.StatusCode, errorContent);
                    throw new HttpRequestException($"Claude API request failed with status {response.StatusCode}: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Claude API raw response: {ResponseContent}", responseContent);
                
                try
                {
                    var claudeResponse = JsonSerializer.Deserialize<ClaudeApiResponse>(responseContent);
                    _logger.LogInformation("Deserialized response - Content count: {Count}", claudeResponse?.Content?.Length ?? 0);
                    
                    if (claudeResponse?.Content != null && claudeResponse.Content.Length > 0)
                    {
                        var result = claudeResponse.Content[0]?.Text ?? string.Empty;
                        _logger.LogInformation("Extracted text length: {Length}", result.Length);
                        return result;
                    }
                    else
                    {
                        _logger.LogError("Claude response has no content array or empty content");
                        return string.Empty;
                    }
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "Failed to deserialize Claude API response: {Response}", responseContent);
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Claude API");
                throw new InvalidOperationException("Claude API call failed and no fallback is configured", ex);
            }
        }

        public async Task<AnalysisResult> AnalyzeIntentAsync(string message, string[]? customIntents = null, string[]? customEntities = null)
        {
            var analysisJson = await AnalyzeTextForFormGenerationAsync(message);
            var analysis = JsonSerializer.Deserialize<dynamic>(analysisJson);
            
            if (analysis == null)
            {
                throw new InvalidOperationException("Failed to deserialize Claude API response");
            }
            
            return new AnalysisResult
            {
                Intent = analysis.GetProperty("intent").GetString() ?? "Generic",
                Confidence = analysis.GetProperty("confidence").GetDouble(),
                AllIntents = new Dictionary<string, double> { { analysis.GetProperty("intent").GetString() ?? "Generic", analysis.GetProperty("confidence").GetDouble() } },
                Entities = new Dictionary<string, string>()
            };
        }

    }

    // Analysis result class for the intent analysis
    public class AnalysisResult
    {
        public string Intent { get; set; }
        public double Confidence { get; set; }
        public Dictionary<string, double> AllIntents { get; set; }
        public Dictionary<string, string> Entities { get; set; }
    }
}
