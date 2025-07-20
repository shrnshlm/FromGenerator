
// Services/CLUService.cs (Alternative using HttpClient)
using System.Text;
using System.Text.Json;

public interface ICLUService
{
    Task<string> AnalyzeConversationAsync(string query);
}

public class CLUService : ICLUService
{
    private readonly HttpClient _httpClient;
    private readonly string _endpoint;
    private readonly string _apiKey;
    private readonly string _projectName;
    private readonly string _deploymentName;

    public CLUService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _endpoint = configuration["CLU:Endpoint"];
        _apiKey = configuration["CLU:ApiKey"];
        _projectName = configuration["CLU:ProjectName"];
        _deploymentName = configuration["CLU:DeploymentName"];
    }

    public async Task<string> AnalyzeConversationAsync(string query)
    {
        var requestBody = new
        {
            analysisInput = new
            {
                conversationItem = new
                {
                    text = query,
                    id = "1",
                    participantId = "1"
                }
            },
            parameters = new
            {
                projectName = _projectName,
                deploymentName = _deploymentName,
                verbose = true,
                stringIndexType = "Utf16CodeUnit"
            },
            kind = "Conversation"
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"{_endpoint.TrimEnd('/')}/language/:analyze-conversations?api-version=2022-10-01-preview";

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = content;
        request.Headers.Add("Ocp-Apim-Subscription-Key", _apiKey);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}

// Models/CLUResponse.cs
public class CLUResponse
{
    public string Query { get; set; }
    public string TopIntent { get; set; }
    public double TopIntentConfidence { get; set; }
    public IDictionary<string, object> Entities { get; set; }
    public IDictionary<string, double> AllIntents { get; set; }
}
