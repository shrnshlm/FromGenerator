using Amazon.Lambda.Core;
using System.Text.Json;
using FromGenerator.Models;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FromGenerator;

public class SimpleWorkingFunction
{
    public Dictionary<string, object> FunctionHandler(Dictionary<string, object> input, ILambdaContext context)
    {
        try
        {
            context.Logger.LogInformation($"Received request: {JsonSerializer.Serialize(input)}");
            
            var headers = new Dictionary<string, object>
            {
                ["Access-Control-Allow-Origin"] = "*",
                ["Access-Control-Allow-Methods"] = "GET, POST, OPTIONS",
                ["Access-Control-Allow-Headers"] = "Content-Type",
                ["Content-Type"] = "application/json"
            };

            // Handle OPTIONS request
            var method = input.GetValueOrDefault("httpMethod", "").ToString();
            if (method == "OPTIONS")
            {
                return new Dictionary<string, object>
                {
                    ["statusCode"] = 200,
                    ["headers"] = headers,
                    ["body"] = ""
                };
            }

            // Handle POST request to generate form
            if (method == "POST")
            {
                var body = input.GetValueOrDefault("body", "").ToString();
                context.Logger.LogInformation($"Body: {body}");
                
                // Parse the request
                FormGenerationRequest? request = null;
                try
                {
                    request = JsonSerializer.Deserialize<FormGenerationRequest>(body ?? "");
                }
                catch (Exception ex)
                {
                    context.Logger.LogError($"JSON parse error: {ex.Message}");
                }

                if (request == null || string.IsNullOrWhiteSpace(request.Text))
                {
                    return new Dictionary<string, object>
                    {
                        ["statusCode"] = 400,
                        ["headers"] = headers,
                        ["body"] = JsonSerializer.Serialize(new { error = "Invalid request", details = "Text is required" })
                    };
                }

                // Generate a simple form without Claude integration
                var form = new GeneratedForm
                {
                    FormId = Guid.NewGuid().ToString(),
                    Title = "Contact Form",
                    Intent = "contact",
                    SubmitUrl = "/api/form/submit",
                    SubmitButtonText = "Submit",
                    Fields = new List<FormField>
                    {
                        new FormField
                        {
                            Name = "name",
                            Label = "Full Name",
                            Type = "text",
                            Required = true,
                            Placeholder = "Enter your full name",
                            Value = ""
                        },
                        new FormField
                        {
                            Name = "email",
                            Label = "Email Address",
                            Type = "email",
                            Required = true,
                            Placeholder = "Enter your email",
                            Value = ""
                        },
                        new FormField
                        {
                            Name = "message",
                            Label = "Message",
                            Type = "textarea",
                            Required = true,
                            Placeholder = "Enter your message",
                            Value = request.Text
                        }
                    }
                };

                return new Dictionary<string, object>
                {
                    ["statusCode"] = 200,
                    ["headers"] = headers,
                    ["body"] = JsonSerializer.Serialize(form)
                };
            }

            return new Dictionary<string, object>
            {
                ["statusCode"] = 404,
                ["headers"] = headers,
                ["body"] = JsonSerializer.Serialize(new { error = "Not Found" })
            };
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Unexpected error: {ex}");
            return new Dictionary<string, object>
            {
                ["statusCode"] = 500,
                ["headers"] = new Dictionary<string, object>
                {
                    ["Access-Control-Allow-Origin"] = "*",
                    ["Content-Type"] = "application/json"
                },
                ["body"] = JsonSerializer.Serialize(new { error = "Internal Server Error", message = ex.Message })
            };
        }
    }
}