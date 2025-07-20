namespace FromGenerator.Models
{

    using System.ComponentModel.DataAnnotations;

    public class FormField
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Type { get; set; } // text, email, date, number, select, etc.
        public bool Required { get; set; }
        public string Placeholder { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public string Value { get; set; }
    }

    public class GeneratedForm
    {
        public string FormId { get; set; }
        public string Title { get; set; }
        public string Intent { get; set; }
        public List<FormField> Fields { get; set; } = new List<FormField>();
        public string SubmitUrl { get; set; }
        public string SubmitButtonText { get; set; }
    }

    public class FormGenerationRequest
    {
        [Required]
        public string Text { get; set; }
        public string UserId { get; set; }
    }

    public class FormSubmissionRequest
    {
        [Required]
        public string FormId { get; set; }
        [Required]
        public Dictionary<string, string> FieldValues { get; set; }
    }

    public class FormSubmissionResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string FormId { get; set; }
        public DateTime SubmittedAt { get; set; }
    }

    public class ErrorResponse
    {
        public string Error { get; set; }
        public string Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
