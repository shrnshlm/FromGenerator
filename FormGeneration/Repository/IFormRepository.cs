using FormGeneration.Models;
using System.Collections.Concurrent;

namespace FormGeneration.Repository
{

    public interface IFormRepository
    {
        Task<bool> SaveFormAsync(GeneratedForm form);
        Task<GeneratedForm> GetFormAsync(string formId);
        Task<bool> DeleteFormAsync(string formId);
        Task<List<GeneratedForm>> GetFormsByUserAsync(string userId);
    }

    public class InMemoryFormRepository : IFormRepository
    {
        private readonly ConcurrentDictionary<string, GeneratedForm> _forms = new();

        public Task<bool> SaveFormAsync(GeneratedForm form)
        {
            _forms.TryAdd(form.FormId, form);
            return Task.FromResult(true);
        }

        public Task<GeneratedForm> GetFormAsync(string formId)
        {
            _forms.TryGetValue(formId, out var form);
            return Task.FromResult(form);
        }

        public Task<bool> DeleteFormAsync(string formId)
        {
            return Task.FromResult(_forms.TryRemove(formId, out _));
        }

        public Task<List<GeneratedForm>> GetFormsByUserAsync(string userId)
        {
            var userForms = _forms.Values.Where(f => f.UserId == userId).ToList();
            return Task.FromResult(userForms);
        }
    }
}
