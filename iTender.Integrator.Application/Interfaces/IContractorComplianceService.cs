using iTender.Integrator.Application.DTOs.Compliance;
using iTender.Integrator.Domain.Entities;

namespace iTender.Integrator.Application.Interfaces
{
    public interface IContractorComplianceService
    {
        Task<ContractorComplianceResult> CheckAsync(Party party, CancellationToken cancellationToken = default);

        /// <summary>Runs CheckAsync and calls party.SetComplianceStatus with the result.</summary>
        Task<Party> CheckAndApplyAsync(Party party, CancellationToken cancellationToken = default);
    }
}
