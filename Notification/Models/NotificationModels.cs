namespace Notification.Models
{

    public class NotificationRequest
    {
        public string Type { get; set; } // Email, SMS, Push, Webhook
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public string Priority { get; set; } = "Normal"; // Low, Normal, High
    }

    public class NotificationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string NotificationId { get; set; }
        public DateTime SentAt { get; set; }
    }
}
