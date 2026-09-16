using iTender.Integrator.Application.DTOs.Crm;

namespace iTender.Integrator.Application.Interfaces
{
    public interface IContractorRepository
    {
        Task<ContractorModel?> GetByCrsNumberAsync(
            string crsNumber,
            CancellationToken cancellationToken = default);
    }
}
