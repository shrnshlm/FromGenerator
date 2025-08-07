namespace FormSubmissionService.Repository
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FormSubmissionService.Models;

    public interface ISubmissionRepository
    {
        Task SaveSubmissionAsync(SubmissionRecord submission);
        Task<SubmissionRecord> GetSubmissionAsync(string submissionId);
        Task<List<SubmissionRecord>> GetSubmissionsByUserAsync(string userId);
    }
}
