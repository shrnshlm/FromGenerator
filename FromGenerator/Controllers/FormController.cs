using FromGenerator.Models;
using FromGenerator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FromGenerator.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class FormController : ControllerBase
    {
        private readonly IFormGeneratorService _formGenerator;

        public FormController(IFormGeneratorService formGenerator)
        {
            _formGenerator = formGenerator;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateForm([FromBody] FormGenerationRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Text))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Error = "Invalid request",
                        Details = "Text is required"
                    });
                }

                var form = await _formGenerator.GenerateFormFromTextAsync(request.Text, request.UserId);
                return Ok(form);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Error = "Internal server error",
                    Details = "An error occurred while generating the form"
                });
            }
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitForm([FromBody] FormSubmissionRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.FormId) ||
                    request.FieldValues == null || !request.FieldValues.Any())
                {
                    return BadRequest(new ErrorResponse
                    {
                        Error = "Invalid request",
                        Details = "Form ID and field values are required"
                    });
                }

                var success = await _formGenerator.ProcessFormSubmissionAsync(request);

                if (success)
                {
                    return Ok(new FormSubmissionResponse
                    {
                        Success = true,
                        Message = "Form submitted successfully",
                        FormId = request.FormId,
                        SubmittedAt = DateTime.UtcNow
                    });
                }

                return BadRequest(new ErrorResponse
                {
                    Error = "Form processing failed",
                    Details = "The form submission could not be processed"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Error = "Internal server error",
                    Details = "An error occurred while processing the form submission"
                });
            }
        }
    }


}


