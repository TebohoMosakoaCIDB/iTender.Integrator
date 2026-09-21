using iTender.Integrator.Application.DTOs.Crm;

namespace iTender.Integrator.Application.Interfaces
{
    public interface IQualifiedContractorFinder
    {
        Task<IReadOnlyCollection<ContractorModel>> FindQualifiedContractorsAsync(
            Guid? provinceId,
            string? requiredGradingDesignationContains,
            CancellationToken cancellationToken = default);
    }
}
