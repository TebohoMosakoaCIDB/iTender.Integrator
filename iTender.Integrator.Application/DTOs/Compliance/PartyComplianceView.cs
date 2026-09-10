using iTender.Integrator.Domain.Enums;

namespace iTender.Integrator.Application.DTOs.Compliance
{
    public sealed record PartyComplianceView(
       string ExternalId,
       string Name,
       string? RegistrationScheme,
       string? RegistrationNumber,
       CidbComplianceStatus ComplianceStatus,
       string Reason);
}
