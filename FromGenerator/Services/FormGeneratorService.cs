using FromGenerator.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using FromGenerator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace FromGenerator.Services
{

    public class FormGeneratorService : IFormGeneratorService
    {
        private readonly ILogger<FormGeneratorService> _logger;
        private readonly IClaudeService _claudeService;

        public FormGeneratorService(ILogger<FormGeneratorService> logger, IClaudeService claudeService)
        {
            _logger = logger;
            _claudeService = claudeService;
        }

        public async Task<GeneratedForm> GenerateFormFromTextAsync(string text, string userId = null)
        {
            try
            {
                _logger.LogInformation("Generating form from text: {Text}", text);

                // Use AI-powered intent detection and entity extraction
                _logger.LogInformation("Using AI-powered analysis for intent detection for text: {Text}", text);
                
                string aiAnalysis;
                try
                {
                    aiAnalysis = await _claudeService.AnalyzeTextForFormGenerationAsync(text);
                    _logger.LogInformation("Claude API returned response length: {Length}", aiAnalysis?.Length ?? 0);
                    
                    if (string.IsNullOrWhiteSpace(aiAnalysis))
                    {
                        _logger.LogError("AI analysis returned empty or null response");
                        throw new InvalidOperationException("AI analysis returned empty response");
                    }
                    
                    _logger.LogDebug("Claude API raw response: {Response}", aiAnalysis);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to get AI analysis from Claude API");
                    throw new InvalidOperationException("Claude API call failed", ex);
                }

                AIAnalysisResponse analysisResult;
                try
                {
                    analysisResult = System.Text.Json.JsonSerializer.Deserialize<AIAnalysisResponse>(aiAnalysis);
                    _logger.LogInformation("Successfully deserialized AI response. Intent: {Intent}", analysisResult?.Intent);
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "Failed to deserialize AI response: {Response}", aiAnalysis);
                    throw new InvalidOperationException($"Failed to parse AI response: {jsonEx.Message}");
                }
                
                if (analysisResult == null || string.IsNullOrWhiteSpace(analysisResult.Intent))
                {
                    _logger.LogError("AI analysis result is null or has no intent");
                    throw new InvalidOperationException("AI analysis failed to detect intent");
                }

                var intent = analysisResult.Intent;
                var entities = ExtractEntitiesFromAIResponse(analysisResult);
                
                _logger.LogInformation("AI detected intent: {Intent} with confidence: {Confidence}", 
                    intent, analysisResult.Confidence);

                var form = new GeneratedForm
                {
                    FormId = Guid.NewGuid().ToString(),
                    Intent = intent,
                    SubmitUrl = "/api/form/submit"
                };

                // Generate form based on detected intent
                switch (intent?.ToLower() ?? "generic")
                {
                    case "bookflight":
                        form = GenerateFlightBookingForm(form, entities, text);
                        break;
                    case "hotelreservation":
                        form = GenerateHotelReservationForm(form, entities, text);
                        break;
                    case "contactus":
                        form = GenerateContactForm(form, entities, text);
                        break;
                    case "registration":
                        form = GenerateRegistrationForm(form, entities, text);
                        break;
                    case "feedback":
                        form = GenerateFeedbackForm(form, entities, text);
                        break;
                    case "appointment":
                        form = GenerateAppointmentForm(form, entities, text);
                        break;
                    default:
                        form = GenerateGenericForm(form, entities, text);
                        break;
                }

                _logger.LogInformation("Generated form '{Title}' with {FieldCount} fields",
                    form.Title, form.Fields.Count);

                return form;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating form from text: {Text}", text);
                throw;
            }
        }


        private GeneratedForm GenerateFlightBookingForm(GeneratedForm form, Dictionary<string, string> entities, string originalText)
        {
            form.Title = "Flight Booking";
            form.SubmitButtonText = "Book Flight";

            form.Fields = new List<FormField>
        {
            new FormField
            {
                Name = "departure",
                Label = "Departure City",
                Type = "text",
                Required = true,
                Value = entities.GetValueOrDefault("departure", ""),
                Placeholder = "Enter departure city"
            },
            new FormField
            {
                Name = "destination",
                Label = "Destination City",
                Type = "text",
                Required = true,
                Value = entities.GetValueOrDefault("destination", ""),
                Placeholder = "Enter destination city"
            },
            new FormField
            {
                Name = "departureDate",
                Label = "Departure Date",
                Type = "date",
                Required = true,
                Value = entities.GetValueOrDefault("date", "")
            },
            new FormField
            {
                Name = "returnDate",
                Label = "Return Date",
                Type = "date",
                Required = false
            },
            new FormField
            {
                Name = "passengers",
                Label = "Number of Passengers",
                Type = "number",
                Required = true,
                Value = entities.GetValueOrDefault("number", "1"),
                Placeholder = "1"
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

            return form;
        }

        private GeneratedForm GenerateHotelReservationForm(GeneratedForm form, Dictionary<string, string> entities, string originalText)
        {
            form.Title = "Hotel Reservation";
            form.SubmitButtonText = "Reserve Room";

            form.Fields = new List<FormField>
        {
            new FormField
            {
                Name = "city",
                Label = "City",
                Type = "text",
                Required = true,
                Value = entities.GetValueOrDefault("city", ""),
                Placeholder = "Enter city"
            },
            new FormField
            {
                Name = "checkIn",
                Label = "Check-in Date",
                Type = "date",
                Required = true,
                Value = entities.GetValueOrDefault("date", "")
            },
            new FormField
            {
                Name = "checkOut",
                Label = "Check-out Date",
                Type = "date",
                Required = true
            },
            new FormField
            {
                Name = "guests",
                Label = "Number of Guests",
                Type = "number",
                Required = true,
                Value = entities.GetValueOrDefault("number", "1")
            },
            new FormField
            {
                Name = "roomType",
                Label = "Room Type",
                Type = "select",
                Required = true,
                Options = new List<string> { "Standard", "Deluxe", "Suite" }
            }
        };

            return form;
        }

        private GeneratedForm GenerateContactForm(GeneratedForm form, Dictionary<string, string> entities, string originalText)
        {
            form.Title = "Contact Us";
            form.SubmitButtonText = "Send Message";

            form.Fields = new List<FormField>
        {
            new FormField
            {
                Name = "name",
                Label = "Full Name",
                Type = "text",
                Required = true,
                Placeholder = "Enter your full name"
            },
            new FormField
            {
                Name = "email",
                Label = "Email Address",
                Type = "email",
                Required = true,
                Placeholder = "Enter your email"
            },
            new FormField
            {
                Name = "phone",
                Label = "Phone Number",
                Type = "tel",
                Required = false,
                Placeholder = "Enter your phone number"
            },
            new FormField
            {
                Name = "subject",
                Label = "Subject",
                Type = "text",
                Required = true,
                Placeholder = "What can we help you with?"
            },
            new FormField
            {
                Name = "message",
                Label = "Message",
                Type = "textarea",
                Required = true,
                Value = originalText,
                Placeholder = "Please describe your inquiry"
            }
        };

            return form;
        }

        private GeneratedForm GenerateRegistrationForm(GeneratedForm form, Dictionary<string, string> entities, string originalText)
        {
            form.Title = "Registration";
            form.SubmitButtonText = "Register";

            form.Fields = new List<FormField>
        {
            new FormField
            {
                Name = "firstName",
                Label = "First Name",
                Type = "text",
                Required = true,
                Placeholder = "Enter your first name"
            },
            new FormField
            {
                Name = "lastName",
                Label = "Last Name",
                Type = "text",
                Required = true,
                Placeholder = "Enter your last name"
            },
            new FormField
            {
                Name = "email",
                Label = "Email Address",
                Type = "email",
                Required = true,
                Placeholder = "Enter your email"
            },
            new FormField
            {
                Name = "phone",
                Label = "Phone Number",
                Type = "tel",
                Required = true,
                Placeholder = "Enter your phone number"
            },
            new FormField
            {
                Name = "newsletter",
                Label = "Subscribe to Newsletter",
                Type = "checkbox",
                Required = false,
                Value = "true"
            }
        };

            return form;
        }

        private GeneratedForm GenerateFeedbackForm(GeneratedForm form, Dictionary<string, string> entities, string originalText)
        {
            form.Title = "Feedback";
            form.SubmitButtonText = "Submit Feedback";

            form.Fields = new List<FormField>
        {
            new FormField
            {
                Name = "name",
                Label = "Your Name",
                Type = "text",
                Required = false,
                Placeholder = "Enter your name (optional)"
            },
            new FormField
            {
                Name = "email",
                Label = "Email Address",
                Type = "email",
                Required = false,
                Placeholder = "Enter your email (optional)"
            },
            new FormField
            {
                Name = "rating",
                Label = "Overall Rating",
                Type = "select",
                Required = true,
                Options = new List<string> { "5 - Excellent", "4 - Good", "3 - Average", "2 - Poor", "1 - Very Poor" }
            },
            new FormField
            {
                Name = "category",
                Label = "Feedback Category",
                Type = "select",
                Required = true,
                Options = new List<string> { "Product Quality", "Customer Service", "Website Experience", "Delivery", "Other" }
            },
            new FormField
            {
                Name = "feedback",
                Label = "Your Feedback",
                Type = "textarea",
                Required = true,
                Value = originalText,
                Placeholder = "Please share your thoughts"
            }
        };

            return form;
        }

        private GeneratedForm GenerateAppointmentForm(GeneratedForm form, Dictionary<string, string> entities, string originalText)
        {
            form.Title = "Schedule Appointment";
            form.SubmitButtonText = "Book Appointment";

            form.Fields = new List<FormField>
        {
            new FormField
            {
                Name = "name",
                Label = "Full Name",
                Type = "text",
                Required = true,
                Placeholder = "Enter your full name"
            },
            new FormField
            {
                Name = "email",
                Label = "Email Address",
                Type = "email",
                Required = true,
                Placeholder = "Enter your email"
            },
            new FormField
            {
                Name = "phone",
                Label = "Phone Number",
                Type = "tel",
                Required = true,
                Placeholder = "Enter your phone number"
            },
            new FormField
            {
                Name = "appointmentDate",
                Label = "Preferred Date",
                Type = "date",
                Required = true,
                Value = entities.GetValueOrDefault("date", "")
            },
            new FormField
            {
                Name = "appointmentTime",
                Label = "Preferred Time",
                Type = "select",
                Required = true,
                Options = new List<string> { "9:00 AM", "10:00 AM", "11:00 AM", "1:00 PM", "2:00 PM", "3:00 PM", "4:00 PM" }
            },
            new FormField
            {
                Name = "reason",
                Label = "Reason for Appointment",
                Type = "textarea",
                Required = true,
                Value = originalText,
                Placeholder = "Please describe the purpose of your appointment"
            }
        };

            return form;
        }

        private GeneratedForm GenerateGenericForm(GeneratedForm form, Dictionary<string, string> entities, string originalText)
        {
            form.Title = "Information Request";
            form.SubmitButtonText = "Submit Request";

            form.Fields = new List<FormField>
        {
            new FormField
            {
                Name = "name",
                Label = "Name",
                Type = "text",
                Required = true,
                Placeholder = "Enter your name"
            },
            new FormField
            {
                Name = "email",
                Label = "Email",
                Type = "email",
                Required = true,
                Placeholder = "Enter your email"
            },
            new FormField
            {
                Name = "subject",
                Label = "Subject",
                Type = "text",
                Required = true,
                Placeholder = "Brief description of your request"
            },
            new FormField
            {
                Name = "message",
                Label = "Details",
                Type = "textarea",
                Required = true,
                Value = originalText,
                Placeholder = "Please provide more details about what you need"
            }
        };

            return form;
        }

        public async Task<bool> ProcessFormSubmissionAsync(FormSubmissionRequest submission)
        {
            try
            {
                _logger.LogInformation("Processing form submission for FormId: {FormId}", submission.FormId);

                // Here you would typically:
                // 1. Validate the form data
                // 2. Save to database
                // 3. Send notifications/emails
                // 4. Call external APIs
                // 5. Process business logic

                // For now, just log the submission
                foreach (var field in submission.FieldValues)
                {
                    _logger.LogInformation("Field: {Key} = {Value}", field.Key, field.Value);
                }

                // Simulate async processing
                await Task.Delay(100);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing form submission");
                return false;
            }
        }

        private Dictionary<string, string> ExtractEntitiesFromAIResponse(AIAnalysisResponse analysisResult)
        {
            var entities = new Dictionary<string, string>();
            
            if (analysisResult.Entities != null)
            {
                foreach (var entity in analysisResult.Entities)
                {
                    entities[entity.Type] = entity.Value;
                }
            }
            
            return entities;
        }
    }

    // Helper classes for AI analysis response
    public class AIAnalysisResponse
    {
        [JsonPropertyName("intent")]
        public string Intent { get; set; }

        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }

        [JsonPropertyName("entities")]
        public List<AIEntity> Entities { get; set; } = new List<AIEntity>();

        [JsonPropertyName("reasoning")]
        public string Reasoning { get; set; }
    }

    public class AIEntity
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }
    }
}
