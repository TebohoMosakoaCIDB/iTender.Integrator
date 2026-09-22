using iTender.Integrator.Application.DTOs.Compliance;
using iTender.Integrator.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace iTender.Integrator.Infrastructure.Services
{
    public sealed class ContractorNotificationService : IContractorNotificationService
    {
        private readonly IQualifiedContractorFinder _finder;
        private readonly INotificationSender _notificationSender;
        private readonly ILogger<ContractorNotificationService> _logger;

        public ContractorNotificationService(
            IQualifiedContractorFinder finder,
            INotificationSender notificationSender,
            ILogger<ContractorNotificationService> logger)
        {
            _finder = finder;
            _notificationSender = notificationSender;
            _logger = logger;
        }

        public async Task<NotifyQualifiedContractorsResult> NotifyQualifiedContractorsAsync(
            Guid? provinceId,
            Guid? classOfWorkTypeId,
            string? requiredGradingDesignationContains,
            string subject,
            string message,
            CancellationToken cancellationToken = default)
        {
            var qualified = await _finder.FindQualifiedContractorsAsync(
                provinceId, classOfWorkTypeId, requiredGradingDesignationContains, cancellationToken);

            var outcomes = new List<ContractorNotificationOutcome>(qualified.Count);

            foreach (var contractor in qualified)
            {
                try
                {
                    var result = await _notificationSender.NotifyAsync(
                        contractor.Name ?? contractor.TradingAs ?? "(unnamed contractor)",
                        contractor.Email,
                        contractor.Phone,
                        subject,
                        message,
                        cancellationToken);

                    outcomes.Add(new ContractorNotificationOutcome(
                        contractor.Name ?? contractor.TradingAs ?? "(unnamed contractor)",
                        contractor.CrsNumber,
                        result.Sent,
                        result.Channel,
                        result.Reason));
                }
                catch (Exception ex)
                {
                    // One contractor's notification failing (bad email format,
                    // gateway timeout once a real one exists) shouldn't stop the
                    // rest of the batch.
                    _logger.LogError(
                        ex, "Failed to notify contractor '{ContractorName}'.", contractor.Name);

                    outcomes.Add(new ContractorNotificationOutcome(
                        contractor.Name ?? contractor.TradingAs ?? "(unnamed contractor)",
                        contractor.CrsNumber,
                        Sent: false,
                        Channel: "none",
                        Reason: $"Notification failed: {ex.Message}"));
                }
            }

            var sentCount = outcomes.Count(o => o.Sent);

            _logger.LogInformation(
                "Contractor notification run complete - {Qualified} qualified, {Sent} actually sent.",
                qualified.Count, sentCount);

            return new NotifyQualifiedContractorsResult(qualified.Count, sentCount, outcomes);
        }
    }
}
