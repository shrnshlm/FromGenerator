using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TextAnalysis.Models;
using TextAnalysis.Services;

namespace TextAnalysis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly ITextAnalysisService _analysisService;

        public AnalysisController(ITextAnalysisService analysisService)
        {
            _analysisService = analysisService;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeText([FromBody] AnalysisRequest request)
        {
            try
            {
                var result = await _analysisService.AnalyzeTextAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("health")]
        public async Task<IActionResult> HealthCheck()
        {
            var isHealthy = await _analysisService.IsHealthyAsync();
            return isHealthy ? Ok(new { status = "healthy" }) : StatusCode(503, new { status = "unhealthy" });
        }
    }
}
