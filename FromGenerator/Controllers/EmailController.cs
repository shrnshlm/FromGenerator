using FromGenerator.Models;
using FromGenerator.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FromGenerator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(EmailService emailService, ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<ActionResult<EmailResponse>> SendEmail([FromBody] EmailRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Received email send request for {To}", request.To);

                var response = await _emailService.SendEmailAsync(request);
                
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(500, response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {To}", request.To);
                return StatusCode(500, new EmailResponse
                {
                    Success = false,
                    Message = "An error occurred while sending the email",
                    ErrorDetails = ex.Message,
                    To = request.To,
                    Subject = request.Subject,
                    SentAt = DateTime.UtcNow
                });
            }
        }

        [HttpPost("send-bulk")]
        public async Task<ActionResult<BulkEmailResponse>> SendBulkEmail([FromBody] BulkEmailRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (request.Recipients.Count == 0)
                {
                    return BadRequest("Recipients list cannot be empty");
                }

                if (request.Recipients.Count > 1000) // Prevent abuse
                {
                    return BadRequest("Maximum 1000 recipients allowed per bulk send");
                }

                _logger.LogInformation("Received bulk email send request for {Count} recipients", request.Recipients.Count);

                var response = await _emailService.SendBulkEmailAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending bulk email");
                return StatusCode(500, "An error occurred while sending bulk emails");
            }
        }

        [HttpPost("send-template")]
        public async Task<ActionResult<EmailResponse>> SendTemplateEmail(
            [FromQuery, Required, EmailAddress] string to,
            [FromQuery, Required] string templateName,
            [FromBody] Dictionary<string, string> parameters)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Sending template email {Template} to {To}", templateName, to);

                var response = await _emailService.SendTemplateEmailAsync(to, templateName, parameters);
                
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(500, response);
                }
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending template email {Template} to {To}", templateName, to);
                return StatusCode(500, "An error occurred while sending the template email");
            }
        }

        [HttpPost("validate-email")]
        public async Task<ActionResult<object>> ValidateEmail([FromBody] string email)
        {
            try
            {
                var isValid = await _emailService.ValidateEmailAddressAsync(email);
                return Ok(new { email, isValid, timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating email {Email}", email);
                return StatusCode(500, "An error occurred while validating the email");
            }
        }

        [HttpGet("test-connection")]
        public async Task<ActionResult<object>> TestConnection()
        {
            try
            {
                var isConnected = await _emailService.TestConnectionAsync();
                return Ok(new 
                { 
                    connected = isConnected, 
                    timestamp = DateTime.UtcNow,
                    message = isConnected ? "SMTP connection successful" : "SMTP connection failed"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing SMTP connection");
                return StatusCode(500, "An error occurred while testing the connection");
            }
        }

        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                status = "healthy",
                service = "Email Service",
                timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("templates")]
        public IActionResult GetAvailableTemplates()
        {
            var templates = new[]
            {
                new { name = "welcome", description = "Welcome email for new users", parameters = new[] { "UserName", "AppName" } },
                new { name = "reset-password", description = "Password reset email", parameters = new[] { "UserName", "ResetLink" } }
            };

            return Ok(new { templates, service = "Email Service" });
        }

        [HttpPost("send-attachment")]
        public async Task<ActionResult<EmailResponse>> SendEmailWithAttachment([FromForm] EmailWithAttachmentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var attachments = new List<EmailAttachment>();
                
                if (request.Files?.Any() == true)
                {
                    foreach (var file in request.Files)
                    {
                        using var stream = new MemoryStream();
                        await file.CopyToAsync(stream);
                        
                        attachments.Add(new EmailAttachment
                        {
                            FileName = file.FileName,
                            Content = stream.ToArray(),
                            ContentType = file.ContentType
                        });
                    }
                }

                var emailRequest = new EmailRequest
                {
                    To = request.To,
                    Cc = request.Cc,
                    Bcc = request.Bcc,
                    Subject = request.Subject,
                    Body = request.Body,
                    IsHtml = request.IsHtml,
                    Attachments = attachments,
                    Priority = request.Priority
                };

                var response = await _emailService.SendEmailAsync(emailRequest);
                
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(500, response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email with attachment to {To}", request.To);
                return StatusCode(500, "An error occurred while sending the email with attachment");
            }
        }
    }
}