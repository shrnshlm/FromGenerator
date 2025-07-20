namespace FormGeneration.Models
{

    public class FormGenerationRequest
    {
        public string Text { get; set; }
        public string UserId { get; set; }
        public AnalysisResult Analysis { get; set; } // From Text Analysis Service
    }

    public class AnalysisResult
    {
        public string Intent { get; set; }
        public double Confidence { get; set; }
        public List<EntityResult> Entities { get; set; }
    }

    public class EntityResult
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public double Confidence { get; set; }
    }

    public class GeneratedForm
    {
        public string FormId { get; set; }
        public string Title { get; set; }
        public string Intent { get; set; }
        public List<FormField> Fields { get; set; }
        public string SubmitUrl { get; set; }
        public string SubmitButtonText { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
    }

    public class FormField
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
        public string Placeholder { get; set; }
        public List<string> Options { get; set; }
        public string Value { get; set; }
        public Dictionary<string, object> Validation { get; set; }
    }
}
