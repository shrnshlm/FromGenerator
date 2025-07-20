
namespace FormSubmissionService.Models
{
    public class FormSubmissionRequest
    {
        public string FormId { get; set; }
        public Dictionary<string, string> FieldValues { get; set; }
        public string UserId { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }

    public class FormSubmissionResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string SubmissionId { get; set; }
        public DateTime ProcessedAt { get; set; }
        public List<string> ValidationErrors { get; set; }
    }

    public class SubmissionRecord  // Changed from FormSubmission to SubmissionRecord
    {
        public string SubmissionId { get; set; }
        public string FormId { get; set; }
        public string UserId { get; set; }
        public Dictionary<string, string> FieldValues { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime ProcessedAt { get; set; }
        public string Status { get; set; } // Pending, Processed, Failed
        public string Intent { get; set; }
    }
}
