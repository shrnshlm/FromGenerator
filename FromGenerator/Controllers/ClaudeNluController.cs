using FromGenerator.Models;
using FromGenerator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace FromGenerator.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class ClaudeNluController : ControllerBase
    {
        private readonly ClaudeService _claudeService;
        private readonly ILogger<ClaudeNluController> _logger;

        public ClaudeNluController(ClaudeService claudeService, ILogger<ClaudeNluController> logger)
        {
            _claudeService = claudeService;
            _logger = logger;
        }

        [HttpPost("analyze")]
        public async Task<ActionResult<NluResponse>> AnalyzeMessage([FromBody] NluRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return BadRequest("Message cannot be empty");
                }

                _logger.LogInformation("Analyzing message with Claude: {Message}", request.Message);

                var analysis = await _claudeService.AnalyzeIntentAsync(
                    request.Message,
                    request.CustomIntents,
                    request.CustomEntities);

                var response = new NluResponse
                {
                    Query = request.Message,
                    TopIntent = analysis.Intent,
                    Confidence = analysis.Confidence,
                    AllIntents = analysis.AllIntents,
                    Entities = analysis.Entities,
                    Response = GenerateResponse(analysis.Intent, analysis.Entities),
                    ProcessedBy = "Claude"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing message: {Message}", request.Message);
                return StatusCode(500, "An error occurred while analyzing your message");
            }
        }

        [HttpPost("predict")]
        public async Task<ActionResult<NluResponse>> PredictIntent([FromBody] NluRequest request)
        {
            // Alias for analyze - compatible with LUIS-style endpoints
            return await AnalyzeMessage(request);
        }

        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                status = "healthy",
                service = "Claude NLU",
                timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("intents")]
        public IActionResult GetAvailableIntents()
        {
            var intents = new[]
            {
            "greeting", "booking", "weather", "cancel", "help", "goodbye",
            "complaint", "compliment", "question", "request", "information"
        };

            return Ok(new { intents, service = "Claude NLU" });
        }

        [HttpGet("entities")]
        public IActionResult GetAvailableEntities()
        {
            var entities = new[]
            {
            "datetime", "location", "person", "number", "email", "phone",
            "product", "service", "duration", "price", "quantity"
        };

            return Ok(new { entities, service = "Claude NLU" });
        }

        private string GenerateResponse(string intent, Dictionary<string, string> entities)
        {
            var response = new StringBuilder();

            switch (intent.ToLower())
            {
                case "greeting":
                    response.Append("Hello! How can I help you today?");
                    break;

                case "booking":
                    response.Append("I can help you with your booking request. ");
                    if (entities.ContainsKey("datetime"))
                        response.Append($"I see you want to book for {entities["datetime"]}. ");
                    if (entities.ContainsKey("location"))
                        response.Append($"The location would be {entities["location"]}. ");
                    response.Append("Please provide any additional details needed.");
                    break;

                case "weather":
                    response.Append("I can help you with weather information. ");
                    if (entities.ContainsKey("location"))
                        response.Append($"You're asking about weather in {entities["location"]}. ");
                    response.Append("Let me get you the current weather information.");
                    break;

                case "cancel":
                    response.Append("I can help you cancel your request. ");
                    if (entities.ContainsKey("service"))
                        response.Append($"You want to cancel {entities["service"]}. ");
                    response.Append("What specifically would you like to cancel?");
                    break;

                case "help":
                    response.Append("I'm here to help! You can ask me about bookings, weather, cancellations, or general questions.");
                    break;

                case "goodbye":
                    response.Append("Goodbye! Have a great day!");
                    break;

                case "complaint":
                    response.Append("I'm sorry to hear you're having an issue. ");
                    if (entities.ContainsKey("service"))
                        response.Append($"Your concern is about {entities["service"]}. ");
                    response.Append("I'll do my best to help resolve this for you.");
                    break;

                case "compliment":
                    response.Append("Thank you for your kind words! I'm glad I could help.");
                    break;

                default:
                    response.Append("I understand you're asking about something. ");
                    if (entities.Any())
                    {
                        response.Append($"I noticed you mentioned {string.Join(", ", entities.Values)}. ");
                    }
                    response.Append("Could you please provide more details about what you need?");
                    break;
            }

            return response.ToString();
        }
    }
}
