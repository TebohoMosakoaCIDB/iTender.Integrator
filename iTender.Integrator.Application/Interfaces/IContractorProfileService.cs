using iTender.Integrator.Application.DTOs.Compliance;

namespace iTender.Integrator.Application.Interfaces
{
    public interface IContractorProfileService
    {
        Task<ContractorFullProfile> GetByCrsNumberAsync(
            string crsNumber, CancellationToken cancellationToken = default);
    }
}
