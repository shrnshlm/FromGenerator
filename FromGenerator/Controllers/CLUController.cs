//using Microsoft.AspNetCore.Mvc;
//using System.Text.Json;
//using FromGenerator.Models;
//using FromGenerator.Services;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System.Text.Json;

//namespace FromGenerator.Controllers
//{


//    // Controllers/CLUController.cs
//    using Microsoft.AspNetCore.Mvc;
//    using System.Text.Json;

//    [ApiController]
//    [Route("api/[controller]")]
//    public class CLUController : ControllerBase
//    {
//        private readonly ICLUService _cluService;

//        public CLUController(ICLUService cluService)
//        {
//            _cluService = cluService;
//        }

//        [HttpPost("analyze")]
//        public async Task<IActionResult> AnalyzeText([FromBody] TextRequest request)
//        {
//            try
//            {
//                var jsonResponse = await _cluService.AnalyzeConversationAsync(request.Text);

//                // Parse the JSON response
//                var jsonDocument = JsonDocument.Parse(jsonResponse);
//                var prediction = jsonDocument.RootElement
//                    .GetProperty("result")
//                    .GetProperty("prediction");

//                var topIntent = prediction.GetProperty("topIntent").GetString();
//                var intents = prediction.GetProperty("intents");
//                var entities = prediction.GetProperty("entities");

//                // Get confidence score for top intent
//                double topIntentConfidence = 0;
//                var allIntents = new Dictionary<string, double>();

//                foreach (var intent in intents.EnumerateArray())
//                {
//                    var intentName = intent.GetProperty("category").GetString();
//                    var confidence = intent.GetProperty("confidenceScore").GetDouble();
//                    allIntents[intentName] = confidence;

//                    if (intentName == topIntent)
//                    {
//                        topIntentConfidence = confidence;
//                    }
//                }

//                // Extract entities
//                var extractedEntities = new Dictionary<string, object>();
//                foreach (var entity in entities.EnumerateArray())
//                {
//                    var category = entity.GetProperty("category").GetString();
//                    var text = entity.GetProperty("text").GetString();
//                    var confidence = entity.GetProperty("confidenceScore").GetDouble();

//                    extractedEntities[category] = new
//                    {
//                        text = text,
//                        confidence = confidence
//                    };
//                }

//                var response = new CLUResponse
//                {
//                    Query = request.Text,
//                    TopIntent = topIntent,
//                    TopIntentConfidence = topIntentConfidence,
//                    Entities = extractedEntities,
//                    AllIntents = allIntents
//                };

//                return Ok(response);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest($"Error processing request: {ex.Message}");
//            }
//        }
//    }


//}
