using Amazon.Lambda.Core;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FromGenerator;

public class TestFunction
{
    public Dictionary<string, object> FunctionHandler(Dictionary<string, object> request, ILambdaContext context)
    {
        context.Logger.LogInformation($"Test function called with: {JsonSerializer.Serialize(request)}");

        return new Dictionary<string, object>
        {
            ["statusCode"] = 200,
            ["headers"] = new Dictionary<string, object>
            {
                ["Access-Control-Allow-Origin"] = "*",
                ["Content-Type"] = "application/json"
            },
            ["body"] = JsonSerializer.Serialize(new { message = "Hello from Lambda!", request = request })
        };
    }
}