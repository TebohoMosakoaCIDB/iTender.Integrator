using iTender.Integrator.Domain.Enums;

namespace iTender.Integrator.Application.DTOs.Compliance
{
    public sealed record ContractorComplianceResult(
        string PartyExternalId,
        string? RegistrationNumber,
        CidbComplianceStatus Status,
        string StatusText,
        string Reason,
        bool CsdSupplierFound,
        bool CrmContractorFound,
        DateTime CheckedAtUtc);
}
