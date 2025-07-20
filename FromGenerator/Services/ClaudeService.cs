using FromGenerator.Configuration;
using FromGenerator.Models;
using FromGenerator.Services;
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
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var claudeResponse = JsonSerializer.Deserialize<ClaudeApiResponse>(responseContent);

                return claudeResponse.Content[0].Text;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Claude API, returning fallback response");
                return GenerateFallbackResponse(text);
            }
        }

        internal dynamic AnalyzeIntentAsync(string formGenerationPrompt)
        {
            throw new NotImplementedException();
        }

        internal dynamic GetResponseAsync(string formGenerationPrompt)
        {
            throw new NotImplementedException();
        }

        internal dynamic SendMessageAsync(string formGenerationPrompt)
        {
            throw new NotImplementedException();
        }

        private string GenerateFallbackResponse(string text)
        {
            var lowerText = text.ToLower();
            string intent = "Generic";
            var entities = new List<object>();

            // Simple keyword-based intent detection
            if (lowerText.Contains("flight") || lowerText.Contains("fly") || (lowerText.Contains("book") && lowerText.Contains("ticket")))
            {
                intent = "BookFlight";
            }
            else if (lowerText.Contains("hotel") || lowerText.Contains("room") || lowerText.Contains("accommodation"))
            {
                intent = "HotelReservation";
            }
            else if (lowerText.Contains("contact") || lowerText.Contains("question") || lowerText.Contains("help"))
            {
                intent = "ContactUs";
            }
            else if (lowerText.Contains("register") || lowerText.Contains("signup") || lowerText.Contains("account"))
            {
                intent = "Registration";
            }
            else if (lowerText.Contains("feedback") || lowerText.Contains("review") || lowerText.Contains("rating"))
            {
                intent = "Feedback";
            }
            else if (lowerText.Contains("appointment") || lowerText.Contains("schedule") || lowerText.Contains("meeting"))
            {
                intent = "Appointment";
            }

            var fallbackResponse = new
            {
                intent = intent,
                confidence = 0.70,
                entities = entities,
                reasoning = "Fallback keyword-based detection"
            };

            return JsonSerializer.Serialize(fallbackResponse);
        }
    }

}
public class ClaudeService : IClaudeService
{
    private readonly HttpClient _httpClient;
    private readonly ClaudeSettings _settings;
    private readonly ILogger<ClaudeService> _logger;

    // Add the missing method definition
    public async Task<AnalysisResult> AnalyzeIntentAsync(string message, string[]? customIntents, string[]? customEntities)
    {
        // Implement the logic for analyzing the intent
        // This is a placeholder implementation
        return new AnalysisResult
        {
            Intent = "defaultIntent",
            Confidence = 0.9,
            // Correct the initialization of the AllIntents dictionary by providing a value for the defaultIntent key.
            AllIntents = new Dictionary<string, double> { { "defaultIntent", 0.9 } },
            Entities = new Dictionary<string, string>()
        };
    }

    public async Task<string> AnalyzeTextForFormGenerationAsync(string text)
    {
        try
        {
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
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var claudeResponse = JsonSerializer.Deserialize<ClaudeApiResponse>(responseContent);

            return claudeResponse.Content[0].Text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Claude API, returning fallback response");
            return GenerateFallbackResponse(text);
        }
    }

    private string GenerateFallbackResponse(string text)
    {
        var lowerText = text.ToLower();
        string intent = "Generic";
        var entities = new List<object>();

        // Simple keyword-based intent detection  
        if (lowerText.Contains("flight") || lowerText.Contains("fly") || (lowerText.Contains("book") && lowerText.Contains("ticket")))
        {
            intent = "BookFlight";
        }
        else if (lowerText.Contains("hotel") || lowerText.Contains("room") || lowerText.Contains("accommodation"))
        {
            intent = "HotelReservation";
        }
        else if (lowerText.Contains("contact") || lowerText.Contains("question") || lowerText.Contains("help"))
        {
            intent = "ContactUs";
        }
        else if (lowerText.Contains("register") || lowerText.Contains("signup") || lowerText.Contains("account"))
        {
            intent = "Registration";
        }
        else if (lowerText.Contains("feedback") || lowerText.Contains("review") || lowerText.Contains("rating"))
        {
            intent = "Feedback";
        }
        else if (lowerText.Contains("appointment") || lowerText.Contains("schedule") || lowerText.Contains("meeting"))
        {
            intent = "Appointment";
        }

        var fallbackResponse = new
        {
            intent = intent,
            confidence = 0.70,
            entities = entities,
            reasoning = "Fallback keyword-based detection"
        };

        return JsonSerializer.Serialize(fallbackResponse);
    }
}

// Define the AnalysisResult class if it doesn't exist
public class AnalysisResult
{
    public string Intent { get; set; }
    public double Confidence { get; set; }
    public Dictionary<string, double> AllIntents { get; set; }
    public Dictionary<string, string> Entities { get; set; }
}
