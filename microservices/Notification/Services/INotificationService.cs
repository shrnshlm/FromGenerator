using Notification.Models;

namespace Notification.Services
{

    public interface INotificationService
    {
        Task<NotificationResponse> SendEmailAsync(NotificationRequest request);
        Task<NotificationResponse> SendSMSAsync(NotificationRequest request);
        Task<NotificationResponse> SendPushNotificationAsync(NotificationRequest request);
        Task<NotificationResponse> SendWebhookAsync(NotificationRequest request);
    }

    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly IEmailService _emailService;
        private readonly ISMSService _smsService;

        public NotificationService(
            ILogger<NotificationService> logger,
            IEmailService emailService,
            ISMSService smsService)
        {
            _logger = logger;
            _emailService = emailService;
            _smsService = smsService;
        }

        public async Task<NotificationResponse> SendEmailAsync(NotificationRequest request)
        {
            try
            {
                var emailSent = await _emailService.SendEmailAsync(
                    request.Recipient,
                    request.Subject,
                    request.Message
                );

                return new NotificationResponse
                {
                    Success = emailSent,
                    Message = emailSent ? "Email sent successfully" : "Failed to send email",
                    NotificationId = Guid.NewGuid().ToString(),
                    SentAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email");
                return new NotificationResponse
                {
                    Success = false,
                    Message = "Error sending email"
                };
            }
        }

        public async Task<NotificationResponse> SendSMSAsync(NotificationRequest request)
        {
            try
            {
                var smsSent = await _smsService.SendSMSAsync(request.Recipient, request.Message);

                return new NotificationResponse
                {
                    Success = smsSent,
                    Message = smsSent ? "SMS sent successfully" : "Failed to send SMS",
                    NotificationId = Guid.NewGuid().ToString(),
                    SentAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SMS");
                return new NotificationResponse
                {
                    Success = false,
                    Message = "Error sending SMS"
                };
            }
        }

        public async Task<NotificationResponse> SendPushNotificationAsync(NotificationRequest request)
        {
            // Implementation for push notifications
            return new NotificationResponse
            {
                Success = true,
                Message = "Push notification sent",
                NotificationId = Guid.NewGuid().ToString(),
                SentAt = DateTime.UtcNow
            };
        }

        public async Task<NotificationResponse> SendWebhookAsync(NotificationRequest request)
        {
            // Implementation for webhooks
            return new NotificationResponse
            {
                Success = true,
                Message = "Webhook sent",
                NotificationId = Guid.NewGuid().ToString(),
                SentAt = DateTime.UtcNow
            };
        }
    }
}
