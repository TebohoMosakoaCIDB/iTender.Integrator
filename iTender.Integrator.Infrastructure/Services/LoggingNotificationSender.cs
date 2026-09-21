using iTender.Integrator.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace iTender.Integrator.Infrastructure.Services
{
    public sealed class LoggingNotificationSender : INotificationSender
    {
        private readonly ILogger<LoggingNotificationSender> _logger;

        public LoggingNotificationSender(ILogger<LoggingNotificationSender> logger)
        {
            _logger = logger;
        }

        public Task<NotificationResult> NotifyAsync(
            string recipientName,
            string? recipientEmail,
            string? recipientPhone,
            string subject,
            string message,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "[No gateway configured] Would notify {RecipientName} ({Email} / {Phone}) - Subject: '{Subject}'",
                recipientName, recipientEmail ?? "no email on file", recipientPhone ?? "no phone on file", subject);

            return Task.FromResult(new NotificationResult(
                Sent: false,
                Channel: "none",
                Reason: "No notification gateway configured yet - logged only, nothing was actually sent."));
        }
    }
}
