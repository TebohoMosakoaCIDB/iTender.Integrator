using iTender.Integrator.Domain.Entities;

namespace iTender.Integrator.Application.Interfaces
{
    public interface IContractorTenderMatchRepository
    {
        Task<bool> ExistsAsync(
            Guid contractorId,
            string ocid,
            string tenderExternalId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            ContractorTenderMatch match,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<ContractorTenderMatch>> GetPendingNotificationsAsync(
            int take = 100,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            ContractorTenderMatch match,
            CancellationToken cancellationToken = default);
    }
}
