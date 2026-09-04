using iTender.Integrator.Domain.Entities;

namespace iTender.Integrator.Application.Interfaces
{
    public interface IContractorRepository
    {
        Task<Contractor?> GetByCidbRegistrationNumberAsync(
            string registrationNumber,
            CancellationToken cancellationToken = default);

        Task<Contractor?> GetByDynamicsAccountIdAsync(
            Guid dynamicsAccountId,
            CancellationToken cancellationToken = default);

        // Point #3 - find contractors eligible to be alerted for a class of work/grade.
        Task<IReadOnlyCollection<Contractor>> GetEligibleForAsync(
            string classOfWork,
            int minGrade,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            Contractor contractor,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            Contractor contractor,
            CancellationToken cancellationToken = default);
    }
}
