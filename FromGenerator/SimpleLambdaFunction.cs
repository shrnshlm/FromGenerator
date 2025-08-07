using Amazon.Lambda.Core;
using System.Text.Json;
using FromGenerator.Services;
using FromGenerator.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using FromGenerator.Configuration;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FromGenerator;

public class SimpleLambdaFunction
{
    private readonly IServiceProvider _serviceProvider;

    public SimpleLambdaFunction()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Production.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.Configure<ClaudeSettings>(configuration.GetSection("Claude"));
        services.Configure<EmailConfig>(configuration.GetSection("EmailConfig"));
        services.AddHttpClient<ClaudeService>();
        services.AddScoped<IClaudeService, ClaudeService>();
        services.AddScoped<IFormGeneratorService, FormGeneratorService>();
        services.AddLogging();
    }

    // Lambda Function URL handler - uses simple dictionary format
    public async Task<Dictionary<string, object>> FunctionHandler(Dictionary<string, object> request, ILambdaContext context)
    {
        try
        {
            context.Logger.LogInformation($"Request received: {JsonSerializer.Serialize(request)}");

            // Extract request details
            var httpMethod = request.GetValueOrDefault("httpMethod", "").ToString();
            var path = request.GetValueOrDefault("path", "").ToString();
            var body = request.GetValueOrDefault("body", "").ToString();

            context.Logger.LogInformation($"Method: {httpMethod}, Path: {path}");

            // Add CORS headers
            var headers = new Dictionary<string, object>
            {
                ["Access-Control-Allow-Origin"] = "*",
                ["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS",
                ["Access-Control-Allow-Headers"] = "Content-Type, Authorization, X-Amz-Date, X-Api-Key, X-Amz-Security-Token",
                ["Content-Type"] = "application/json"
            };

            // Handle preflight OPTIONS request
            if (httpMethod == "OPTIONS")
            {
                return new Dictionary<string, object>
                {
                    ["statusCode"] = 200,
                    ["headers"] = headers,
                    ["body"] = ""
                };
            }

            // Route the request
            if (httpMethod == "POST" && (path.EndsWith("/api/form/generate") || path.Contains("form/generate")))
            {
                return await HandleGenerateForm(body, headers, context);
            }
            else if (httpMethod == "POST" && (path.EndsWith("/api/form/submit") || path.Contains("form/submit")))
            {
                return await HandleSubmitForm(body, headers, context);
            }
            else
            {
                return new Dictionary<string, object>
                {
                    ["statusCode"] = 404,
                    ["headers"] = headers,
                    ["body"] = JsonSerializer.Serialize(new { error = "Not Found", message = $"Endpoint not found: {httpMethod} {path}" })
                };
            }
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error: {ex}");
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

    private async Task<Dictionary<string, object>> HandleGenerateForm(string body, Dictionary<string, object> headers, ILambdaContext context)
    {
        try
        {
            context.Logger.LogInformation($"HandleGenerateForm called with body: {body}");
            
            var requestBody = JsonSerializer.Deserialize<FormGenerationRequest>(body);
            if (requestBody == null || string.IsNullOrWhiteSpace(requestBody.Text))
            {
                return new Dictionary<string, object>
                {
                    ["statusCode"] = 400,
                    ["headers"] = headers,
                    ["body"] = JsonSerializer.Serialize(new { error = "Invalid request", details = "Text is required" })
                };
            }

            var formGenerator = _serviceProvider.GetRequiredService<IFormGeneratorService>();
            var form = await formGenerator.GenerateFormFromTextAsync(requestBody.Text, requestBody.UserId);

            return new Dictionary<string, object>
            {
                ["statusCode"] = 200,
                ["headers"] = headers,
                ["body"] = JsonSerializer.Serialize(form)
            };
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error in HandleGenerateForm: {ex}");
            return new Dictionary<string, object>
            {
                ["statusCode"] = 500,
                ["headers"] = headers,
                ["body"] = JsonSerializer.Serialize(new { error = "Internal server error", details = ex.Message })
            };
        }
    }

    private async Task<Dictionary<string, object>> HandleSubmitForm(string body, Dictionary<string, object> headers, ILambdaContext context)
    {
        try
        {
            context.Logger.LogInformation($"HandleSubmitForm called with body: {body}");
            
            var requestBody = JsonSerializer.Deserialize<FormSubmissionRequest>(body);
            if (requestBody == null || string.IsNullOrWhiteSpace(requestBody.FormId))
            {
                return new Dictionary<string, object>
                {
                    ["statusCode"] = 400,
                    ["headers"] = headers,
                    ["body"] = JsonSerializer.Serialize(new { error = "Invalid request", details = "Form ID is required" })
                };
            }

            var formGenerator = _serviceProvider.GetRequiredService<IFormGeneratorService>();
            var success = await formGenerator.ProcessFormSubmissionAsync(requestBody);

            if (success)
            {
                return new Dictionary<string, object>
                {
                    ["statusCode"] = 200,
                    ["headers"] = headers,
                    ["body"] = JsonSerializer.Serialize(new FormSubmissionResponse
                    {
                        Success = true,
                        Message = "Form submitted successfully",
                        FormId = requestBody.FormId,
                        SubmittedAt = DateTime.UtcNow
                    })
                };
            }

            return new Dictionary<string, object>
            {
                ["statusCode"] = 400,
                ["headers"] = headers,
                ["body"] = JsonSerializer.Serialize(new { error = "Form processing failed", details = "The form submission could not be processed" })
            };
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error in HandleSubmitForm: {ex}");
            return new Dictionary<string, object>
            {
                ["statusCode"] = 500,
                ["headers"] = headers,
                ["body"] = JsonSerializer.Serialize(new { error = "Internal server error", details = ex.Message })
            };
        }
    }
}