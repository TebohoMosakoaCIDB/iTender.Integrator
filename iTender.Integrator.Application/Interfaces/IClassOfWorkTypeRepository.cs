using iTender.Integrator.Application.DTOs.Crm;

namespace iTender.Integrator.Application.Interfaces
{
    public interface IClassOfWorkTypeRepository
    {
        Task<IReadOnlyList<ClassOfWorkTypeModel>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<ClassOfWorkTypeModel?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
