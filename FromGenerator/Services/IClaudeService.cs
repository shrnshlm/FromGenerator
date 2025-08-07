namespace FromGenerator.Services
{
    public interface IClaudeService
    {
        Task<string> AnalyzeTextForFormGenerationAsync(string text);
        Task<AnalysisResult> AnalyzeIntentAsync(string message, string[]? customIntents = null, string[]? customEntities = null);
    }
}
