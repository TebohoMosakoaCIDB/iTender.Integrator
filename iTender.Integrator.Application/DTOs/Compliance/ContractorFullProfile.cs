using iTender.Integrator.Application.DTOs.Crm;
using iTender.Integrator.Domain.Entities.Csd;

namespace iTender.Integrator.Application.DTOs.Compliance
{
    public sealed record ContractorFullProfile(
        string CrsNumber,
        ContractorModel? Crm,
        CsdSupplier? Csd,
        string? CrmLookupReason,
        string? CsdLookupReason);
}
