using iTender.Integrator.Domain.Enums;

namespace iTender.Integrator.Application.DTOs.Compliance
{

    public sealed record ContractorComplianceResult(
        string PartyExternalId,
        string? CRSNumber,
        string? MAAANumber,
        CidbComplianceStatus Status,
        string StatusText,
        string Reason,
        bool CsdSupplierFound,
        bool CrmContractorFound,
        DateTime CheckedAtUtc);

    /// <summary>
    /// A single display item on the compliance dashboard.
    /// </summary>
    public sealed class ComplianceCheckItem
    {
        public string Key { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? Reference { get; init; }
        public DateTime? ExpiryDate { get; init; }
        public bool IsPassed { get; init; }
        public string? Detail { get; init; }
    }
}
