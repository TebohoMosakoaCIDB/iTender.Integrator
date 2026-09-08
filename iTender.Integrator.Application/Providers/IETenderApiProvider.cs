using iTender.Integrator.Application.DTOs.Etender;

namespace iTender.Integrator.Application.Providers
{
    public interface IETenderApiProvider
    {
        Task<List<ExternalTenderModel>> GetTendersAsync(CancellationToken ct = default);
    }
}
