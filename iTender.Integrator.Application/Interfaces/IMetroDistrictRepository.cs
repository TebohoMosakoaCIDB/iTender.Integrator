using iTender.Integrator.Application.DTOs.Crm;

namespace iTender.Integrator.Application.Interfaces
{
    public interface IMetroDistrictRepository
    {
        Task<IReadOnlyList<MetroDistrictModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MetroDistrictModel>> GetByProvinceAsync(
            Guid provinceId, CancellationToken cancellationToken = default);

        Task<MetroDistrictModel?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
