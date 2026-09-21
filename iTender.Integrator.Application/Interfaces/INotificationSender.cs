namespace iTender.Integrator.Application.Interfaces
{
    public sealed record NotificationResult(bool Sent, string Channel, string? Reason);

    public interface INotificationSender
    {
        Task<NotificationResult> NotifyAsync(
            string recipientName,
            string? recipientEmail,
            string? recipientPhone,
            string subject,
            string message,
            CancellationToken cancellationToken = default);
    }
}
