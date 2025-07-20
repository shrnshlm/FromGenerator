using FromGenerator.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace FromGenerator.Services
{
    public class EmailService
    {
        private readonly EmailConfig _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailConfig> config, ILogger<EmailService> logger)
        {
            _config = config.Value;
            _logger = logger;

            // Validate configuration
            ValidateConfiguration();
        }

        public async Task<EmailResponse> SendEmailAsync(EmailRequest request)
        {
            var response = new EmailResponse
            {
                To = request.To,
                Subject = request.Subject,
                SentAt = DateTime.UtcNow
            };

            try
            {
                _logger.LogInformation("Sending email to {To} with subject: {Subject}", request.To, request.Subject);

                using var client = CreateSmtpClient();
                using var message = CreateMailMessage(request);

                await client.SendMailAsync(message);

                response.Success = true;
                response.MessageId = Guid.NewGuid().ToString();
                response.Message = "Email sent successfully";

                _logger.LogInformation("Email sent successfully to {To}", request.To);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to send email";
                response.ErrorDetails = ex.Message;

                _logger.LogError(ex, "Failed to send email to {To}", request.To);
            }

            return response;
        }

        public async Task<BulkEmailResponse> SendBulkEmailAsync(BulkEmailRequest request)
        {
            var response = new BulkEmailResponse
            {
                TotalEmails = request.Recipients.Count,
                StartedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Starting bulk email send to {Count} recipients", request.Recipients.Count);

            try
            {
                var batches = request.Recipients
                    .Select((email, index) => new { email, index })
                    .GroupBy(x => x.index / request.BatchSize)
                    .Select(g => g.Select(x => x.email).ToList())
                    .ToList();

                foreach (var batch in batches)
                {
                    var tasks = batch.Select(async email =>
                    {
                        var emailRequest = new EmailRequest
                        {
                            To = email,
                            Subject = request.Subject,
                            Body = request.Body,
                            IsHtml = request.IsHtml,
                            Attachments = request.Attachments
                        };

                        return await SendEmailAsync(emailRequest);
                    });

                    var results = await Task.WhenAll(tasks);
                    response.Results.AddRange(results);

                    // Delay between batches to avoid overwhelming the SMTP server
                    if (request.DelayBetweenBatchesMs > 0)
                    {
                        await Task.Delay(request.DelayBetweenBatchesMs);
                    }
                }

                response.SuccessCount = response.Results.Count(r => r.Success);
                response.FailureCount = response.Results.Count(r => !r.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk email send");
                throw;
            }
            finally
            {
                response.CompletedAt = DateTime.UtcNow;
                response.Duration = response.CompletedAt - response.StartedAt;

                _logger.LogInformation("Bulk email completed. Success: {Success}, Failed: {Failed}, Duration: {Duration}",
                    response.SuccessCount, response.FailureCount, response.Duration);
            }

            return response;
        }

        public async Task<EmailResponse> SendTemplateEmailAsync(string to, string templateName, Dictionary<string, string> parameters)
        {
            try
            {
                var template = await LoadEmailTemplateAsync(templateName);
                var processedTemplate = ProcessTemplate(template, parameters);

                var request = new EmailRequest
                {
                    To = to,
                    Subject = processedTemplate.Subject,
                    Body = processedTemplate.Body,
                    IsHtml = processedTemplate.IsHtml
                };

                return await SendEmailAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send template email {Template} to {To}", templateName, to);
                throw;
            }
        }

        public Task<bool> ValidateEmailAddressAsync(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return Task.FromResult(addr.Address == email);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public Task<bool> TestConnectionAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    using var client = CreateSmtpClient();
                    // Note: Some SMTP servers don't support testing connection without sending
                    _logger.LogInformation("SMTP connection test successful");
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "SMTP connection test failed");
                    return false;
                }
            });
        }

        private SmtpClient CreateSmtpClient()
        {
            var client = new SmtpClient(_config.SmtpServer, _config.Port)
            {
                EnableSsl = _config.EnableSsl,
                Timeout = _config.TimeoutSeconds * 1000,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_config.Username, _config.Password)
            };

            return client;
        }

        private MailMessage CreateMailMessage(EmailRequest request)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_config.FromEmail, _config.FromName),
                Subject = request.Subject,
                Body = request.Body,
                IsBodyHtml = request.IsHtml,
                Priority = request.Priority switch
                {
                    EmailPriority.Low => MailPriority.Low,
                    EmailPriority.High => MailPriority.High,
                    _ => MailPriority.Normal
                }
            };

            // Add recipients
            message.To.Add(request.To);

            // Add CC recipients
            if (!string.IsNullOrWhiteSpace(request.Cc))
            {
                var ccAddresses = request.Cc.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var cc in ccAddresses)
                {
                    message.CC.Add(cc.Trim());
                }
            }

            // Add BCC recipients
            if (!string.IsNullOrWhiteSpace(request.Bcc))
            {
                var bccAddresses = request.Bcc.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var bcc in bccAddresses)
                {
                    message.Bcc.Add(bcc.Trim());
                }
            }

            // Add attachments
            if (request.Attachments?.Any() == true)
            {
                foreach (var attachment in request.Attachments)
                {
                    var stream = new MemoryStream(attachment.Content);
                    var mailAttachment = new Attachment(stream, attachment.FileName, attachment.ContentType);
                    message.Attachments.Add(mailAttachment);
                }
            }

            // Add custom headers
            if (request.CustomHeaders?.Any() == true)
            {
                foreach (var header in request.CustomHeaders)
                {
                    message.Headers.Add(header.Key, header.Value);
                }
            }

            return message;
        }

        private void ValidateConfiguration()
        {
            if (string.IsNullOrWhiteSpace(_config.SmtpServer))
                throw new InvalidOperationException("SMTP server is not configured");

            if (string.IsNullOrWhiteSpace(_config.Username))
                throw new InvalidOperationException("SMTP username is not configured");

            if (string.IsNullOrWhiteSpace(_config.Password))
                throw new InvalidOperationException("SMTP password is not configured");

            if (string.IsNullOrWhiteSpace(_config.FromEmail))
                throw new InvalidOperationException("From email is not configured");

            _logger.LogInformation("Email service initialized with SMTP server: {Server}:{Port}", _config.SmtpServer, _config.Port);
        }

        private async Task<EmailTemplate> LoadEmailTemplateAsync(string templateName)
        {
            // This is a placeholder - implement template loading from file system, database, etc.
            await Task.CompletedTask;
            
            return templateName.ToLower() switch
            {
                "welcome" => new EmailTemplate
                {
                    Subject = "Welcome to {{AppName}}!",
                    Body = "<h1>Welcome {{UserName}}!</h1><p>Thank you for joining {{AppName}}.</p>",
                    IsHtml = true
                },
                "reset-password" => new EmailTemplate
                {
                    Subject = "Password Reset Request",
                    Body = "<p>Hello {{UserName}},</p><p>Click <a href='{{ResetLink}}'>here</a> to reset your password.</p>",
                    IsHtml = true
                },
                _ => throw new ArgumentException($"Template '{templateName}' not found")
            };
        }

        private EmailTemplate ProcessTemplate(EmailTemplate template, Dictionary<string, string> parameters)
        {
            var processedTemplate = new EmailTemplate
            {
                Subject = template.Subject,
                Body = template.Body,
                IsHtml = template.IsHtml
            };

            foreach (var parameter in parameters)
            {
                var placeholder = $"{{{{{parameter.Key}}}}}";
                processedTemplate.Subject = processedTemplate.Subject.Replace(placeholder, parameter.Value);
                processedTemplate.Body = processedTemplate.Body.Replace(placeholder, parameter.Value);
            }

            return processedTemplate;
        }

        private class EmailTemplate
        {
            public string Subject { get; set; } = string.Empty;
            public string Body { get; set; } = string.Empty;
            public bool IsHtml { get; set; } = false;
        }
    }
}