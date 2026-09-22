using iTender.Integrator.Application.DTOs.Crm;

namespace iTender.Integrator.Application.Interfaces
{
    public interface IQualifiedContractorFinder
    {
        Task<IReadOnlyCollection<ContractorModel>> FindQualifiedContractorsAsync(
            Guid? provinceId,
            Guid? classOfWorkTypeId,
            string? requiredGradingDesignationContains,
            CancellationToken cancellationToken = default);
    }
}
