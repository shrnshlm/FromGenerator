using System.ComponentModel.DataAnnotations;

namespace FromGenerator.Models
{
    public class EmailConfig
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
        public int TimeoutSeconds { get; set; } = 30;
    }

    public class EmailRequest
    {
        [Required]
        [EmailAddress]
        public string To { get; set; } = string.Empty;
        
        public string? Cc { get; set; }
        
        public string? Bcc { get; set; }
        
        [Required]
        public string Subject { get; set; } = string.Empty;
        
        [Required]
        public string Body { get; set; } = string.Empty;
        
        public bool IsHtml { get; set; } = false;
        
        public List<EmailAttachment>? Attachments { get; set; }
        
        public Dictionary<string, string>? CustomHeaders { get; set; }
        
        public EmailPriority Priority { get; set; } = EmailPriority.Normal;
    }

    public class EmailAttachment
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = "application/octet-stream";
    }

    public class EmailResponse
    {
        public bool Success { get; set; }
        public string MessageId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string? ErrorDetails { get; set; }
    }

    public class BulkEmailRequest
    {
        [Required]
        public List<string> Recipients { get; set; } = new();
        
        [Required]
        public string Subject { get; set; } = string.Empty;
        
        [Required]
        public string Body { get; set; } = string.Empty;
        
        public bool IsHtml { get; set; } = false;
        
        public List<EmailAttachment>? Attachments { get; set; }
        
        public int BatchSize { get; set; } = 10;
        
        public int DelayBetweenBatchesMs { get; set; } = 1000;
    }

    public class BulkEmailResponse
    {
        public int TotalEmails { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<EmailResponse> Results { get; set; } = new();
        public DateTime StartedAt { get; set; }
        public DateTime CompletedAt { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public enum EmailPriority
    {
        Low = 0,
        Normal = 1,
        High = 2
    }

    public class EmailWithAttachmentRequest
    {
        [Required, EmailAddress]
        public string To { get; set; } = string.Empty;
        
        public string? Cc { get; set; }
        
        public string? Bcc { get; set; }
        
        [Required]
        public string Subject { get; set; } = string.Empty;
        
        [Required]
        public string Body { get; set; } = string.Empty;
        
        public bool IsHtml { get; set; } = false;
        
        public EmailPriority Priority { get; set; } = EmailPriority.Normal;
        
        public List<IFormFile>? Files { get; set; }
    }
}