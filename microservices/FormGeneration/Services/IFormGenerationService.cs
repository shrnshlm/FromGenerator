using FormGeneration.Models;
using FormGeneration.Repository;
using System.Text.Json;

namespace FormGeneration.Services
{
    public interface IFormGenerationService
    {
        Task<GeneratedForm> GenerateFormAsync(FormGenerationRequest request);
        Task<GeneratedForm> GetFormAsync(string formId);
        Task<bool> DeleteFormAsync(string formId);
    }

    public class FormGenerationService : IFormGenerationService
    {
        private readonly HttpClient _httpClient;
        private readonly IFormRepository _formRepository;
        private readonly ILogger<FormGenerationService> _logger;

        public FormGenerationService(
            HttpClient httpClient,
            IFormRepository formRepository,
            ILogger<FormGenerationService> logger)
        {
            _httpClient = httpClient;
            _formRepository = formRepository;
            _logger = logger;
        }

        public async Task<GeneratedForm> GenerateFormAsync(FormGenerationRequest request)
        {
            try
            {
                // If analysis not provided, call Text Analysis Service
                if (request.Analysis == null)
                {
                    request.Analysis = await GetTextAnalysis(request.Text, request.UserId);
                }

                var form = new GeneratedForm
                {
                    FormId = Guid.NewGuid().ToString(),
                    Intent = request.Analysis.Intent,
                    CreatedAt = DateTime.UtcNow,
                    UserId = request.UserId,
                    SubmitUrl = "/api/form/submit"
                };

                // Generate fields based on intent
                form.Fields = GenerateFieldsForIntent(request.Analysis.Intent, request.Analysis.Entities);
                form.Title = GetTitleForIntent(request.Analysis.Intent);
                form.SubmitButtonText = GetSubmitButtonText(request.Analysis.Intent);

                // Save form to repository
                await _formRepository.SaveFormAsync(form);

                return form;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating form for text: {Text}", request.Text);
                throw;
            }
        }

        private async Task<AnalysisResult> GetTextAnalysis(string text, string userId)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    "https://localhost:7001/api/analysis/analyze",
                    new { Text = text, UserId = userId }
                );

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<AnalysisResult>(content);
                }

                throw new Exception("Failed to get text analysis");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling text analysis service");
                throw;
            }
        }

        private List<FormField> GenerateFieldsForIntent(string intent, List<EntityResult> entities)
        {
            return intent.ToLower() switch
            {
                "bookflight" => GenerateFlightFields(entities),
                "hotelreservation" => GenerateHotelFields(entities),
                "contactus" => GenerateContactFields(entities),
                "registration" => GenerateRegistrationFields(entities),
                "feedback" => GenerateFeedbackFields(entities),
                "appointment" => GenerateAppointmentFields(entities),
                _ => GenerateGenericFields(entities)
            };
        }

        private List<FormField> GenerateFlightFields(List<EntityResult> entities)
        {
            return new List<FormField>
            {
                new FormField
                {
                    Name = "departure",
                    Label = "Departure City",
                    Type = "text",
                    Required = true,
                    Placeholder = "Enter departure city",
                    Value = GetEntityValue(entities, "departure")
                },
                new FormField
                {
                    Name = "destination",
                    Label = "Destination City",
                    Type = "text",
                    Required = true,
                    Placeholder = "Enter destination city",
                    Value = GetEntityValue(entities, "destination")
                },
                new FormField
                {
                    Name = "departureDate",
                    Label = "Departure Date",
                    Type = "date",
                    Required = true,
                    Value = GetEntityValue(entities, "date")
                },
                new FormField
                {
                    Name = "passengers",
                    Label = "Number of Passengers",
                    Type = "number",
                    Required = true,
                    Value = GetEntityValue(entities, "number") ?? "1",
                    Validation = new Dictionary<string, object> { { "min", 1 }, { "max", 10 } }
                },
                new FormField
                {
                    Name = "class",
                    Label = "Travel Class",
                    Type = "select",
                    Required = true,
                    Options = new List<string> { "Economy", "Business", "First Class" },
                    Value = "Economy"
                }
            };
        }

        private string GetEntityValue(List<EntityResult> entities, string entityType)
        {
            return entities?.FirstOrDefault(e => e.Type.ToLower() == entityType.ToLower())?.Value ?? "";
        }

        private string GetTitleForIntent(string intent)
        {
            return intent.ToLower() switch
            {
                "bookflight" => "Flight Booking",
                "hotelreservation" => "Hotel Reservation",
                "contactus" => "Contact Us",
                "registration" => "Registration",
                "feedback" => "Feedback",
                "appointment" => "Schedule Appointment",
                _ => "Information Request"
            };
        }

        private string GetSubmitButtonText(string intent)
        {
            return intent.ToLower() switch
            {
                "bookflight" => "Book Flight",
                "hotelreservation" => "Reserve Room",
                "contactus" => "Send Message",
                "registration" => "Register",
                "feedback" => "Submit Feedback",
                "appointment" => "Book Appointment",
                _ => "Submit"
            };
        }

        // Add other field generation methods...
        private List<FormField> GenerateHotelFields(List<EntityResult> entities) => new();
        private List<FormField> GenerateContactFields(List<EntityResult> entities) => new();
        private List<FormField> GenerateRegistrationFields(List<EntityResult> entities) => new();
        private List<FormField> GenerateFeedbackFields(List<EntityResult> entities) => new();
        private List<FormField> GenerateAppointmentFields(List<EntityResult> entities) => new();
        private List<FormField> GenerateGenericFields(List<EntityResult> entities) => new();

        public async Task<GeneratedForm> GetFormAsync(string formId)
        {
            return await _formRepository.GetFormAsync(formId);
        }

        public async Task<bool> DeleteFormAsync(string formId)
        {
            return await _formRepository.DeleteFormAsync(formId);
        }
    }
}
