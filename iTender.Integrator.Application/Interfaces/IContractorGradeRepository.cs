using iTender.Integrator.Application.DTOs.Crm;

namespace iTender.Integrator.Application.Interfaces
{
    public interface IContractorGradeRepository
    {
        Task<ContractorModel?> GetByCsdNumberAsync(
            string csdNumber,
            CancellationToken cancellationToken = default);
    }
}
