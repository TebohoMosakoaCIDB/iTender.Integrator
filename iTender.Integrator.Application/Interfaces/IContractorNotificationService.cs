using iTender.Integrator.Application.DTOs.Compliance;

namespace iTender.Integrator.Application.Interfaces
{
    public interface IContractorNotificationService
    {
        Task<NotifyQualifiedContractorsResult> NotifyQualifiedContractorsAsync(
            Guid? provinceId,
            Guid? classOfWorkTypeId,
            string? requiredGradingDesignationContains,
            string subject,
            string message,
            CancellationToken cancellationToken = default);
    }
}
