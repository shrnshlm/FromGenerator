using FromGenerator.Models;

namespace FromGenerator.Services
{

    public interface IFormGeneratorService
    {
        Task<GeneratedForm> GenerateFormFromTextAsync(string text, string userId = null);
        Task<bool> ProcessFormSubmissionAsync(FormSubmissionRequest submission);
    }

}
