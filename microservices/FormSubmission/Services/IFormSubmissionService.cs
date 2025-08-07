using FormSubmissionService.Models;
using FormSubmissionService.Repository;
using System.Text.Json;


namespace FormSubmissionService.Services
{
    public interface IFormSubmissionService
    {
        Task<FormSubmissionResponse> ProcessSubmissionAsync(FormSubmissionRequest request);
        Task<SubmissionRecord> GetSubmissionAsync(string submissionId);
        Task<List<SubmissionRecord>> GetSubmissionsByUserAsync(string userId);
    }

    public class FormSubmissionService : IFormSubmissionService
    {
        private readonly HttpClient _httpClient;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly ILogger<FormSubmissionService> _logger;

        public FormSubmissionService(
            HttpClient httpClient,
            ISubmissionRepository submissionRepository,
            ILogger<FormSubmissionService> logger)
        {
            _httpClient = httpClient;
            _submissionRepository = submissionRepository;
            _logger = logger;
        }

        public async Task<FormSubmissionResponse> ProcessSubmissionAsync(FormSubmissionRequest request)
        {
            try
            {
                var submissionId = Guid.NewGuid().ToString();

                // Get form structure from Form Generation Service
                var form = await GetFormStructure(request.FormId);
                if (form == null)
                {
                    return new FormSubmissionResponse
                    {
                        Success = false,
                        Message = "Form not found",
                        ValidationErrors = new List<string> { "Invalid form ID" }
                    };
                }

                // Validate submission
                var validationErrors = ValidateSubmission(form, request.FieldValues);
                if (validationErrors.Any())
                {
                    return new FormSubmissionResponse
                    {
                        Success = false,
                        Message = "Validation failed",
                        ValidationErrors = validationErrors
                    };
                }

                // Save submission
                var submission = new SubmissionRecord  // Changed from FormSubmission to SubmissionRecord
                {
                    SubmissionId = submissionId,
                    FormId = request.FormId,
                    UserId = request.UserId,
                    FieldValues = request.FieldValues,
                    SubmittedAt = request.SubmittedAt,
                    ProcessedAt = DateTime.UtcNow,
                    Status = "Processed",
                    Intent = form.Intent
                };

                await _submissionRepository.SaveSubmissionAsync(submission);

                // Process business logic based on intent
                await ProcessBusinessLogic(submission);

                // Send notifications
                await SendNotifications(submission);

                return new FormSubmissionResponse
                {
                    Success = true,
                    Message = "Form submitted successfully",
                    SubmissionId = submissionId,
                    ProcessedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing form submission");
                return new FormSubmissionResponse
                {
                    Success = false,
                    Message = "Internal server error"
                };
            }
        }

        private async Task<GeneratedForm> GetFormStructure(string formId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7002/api/form/{formId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<GeneratedForm>(content);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting form structure");
                return null;
            }
        }

        private List<string> ValidateSubmission(GeneratedForm form, Dictionary<string, string> fieldValues)
        {
            var errors = new List<string>();

            foreach (var field in form.Fields.Where(f => f.Required))
            {
                if (!fieldValues.ContainsKey(field.Name) || string.IsNullOrWhiteSpace(fieldValues[field.Name]))
                {
                    errors.Add($"{field.Label} is required");
                }
            }

            // Add more validation logic here

            return errors;
        }

        private async Task ProcessBusinessLogic(SubmissionRecord submission)  // Changed parameter type
        {
            // Process based on intent
            switch (submission.Intent.ToLower())
            {
                case "bookflight":
                    await ProcessFlightBooking(submission);
                    break;
                case "hotelreservation":
                    await ProcessHotelReservation(submission);
                    break;
                case "contactus":
                    await ProcessContactRequest(submission);
                    break;
                    // Add more cases
            }
        }

        private async Task ProcessFlightBooking(SubmissionRecord submission)  // Changed parameter type
        {
            // Integration with flight booking systems
            _logger.LogInformation("Processing flight booking for submission {SubmissionId}", submission.SubmissionId);
            // Add actual flight booking logic
        }

        private async Task ProcessHotelReservation(SubmissionRecord submission)  // Changed parameter type
        {
            // Integration with hotel booking systems
            _logger.LogInformation("Processing hotel reservation for submission {SubmissionId}", submission.SubmissionId);
            // Add actual hotel booking logic
        }

        private async Task ProcessContactRequest(SubmissionRecord submission)  // Changed parameter type
        {
            // Route to appropriate support team
            _logger.LogInformation("Processing contact request for submission {SubmissionId}", submission.SubmissionId);
            // Add actual contact processing logic
        }

        private async Task SendNotifications(SubmissionRecord submission)  // Changed parameter type
        {
            try
            {
                // Call Notification Service
                await _httpClient.PostAsJsonAsync("https://localhost:7004/api/notification/send", new
                {
                    Type = "FormSubmission",
                    SubmissionId = submission.SubmissionId,
                    UserId = submission.UserId,
                    Intent = submission.Intent,
                    FieldValues = submission.FieldValues
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notifications");
            }
        }

        public async Task<SubmissionRecord> GetSubmissionAsync(string submissionId)  // Changed return type
        {
            return await _submissionRepository.GetSubmissionAsync(submissionId);
        }

        public async Task<List<SubmissionRecord>> GetSubmissionsByUserAsync(string userId)  // Changed return type
        {
            return await _submissionRepository.GetSubmissionsByUserAsync(userId);
        }
    }
}