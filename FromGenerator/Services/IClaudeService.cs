namespace FromGenerator.Services
{
    public interface IClaudeService
    {
        Task<string> AnalyzeTextForFormGenerationAsync(string text);
    }
}
