using iTender.Integrator.Application.DTOs.Ocds;

namespace iTender.Integrator.Application.Interfaces
{
    public interface IOcdsApiClient
    {
        Task<OcdsReleasePackageDto> GetReleasesAsync(
            int PageNumber,
            int PageSize,
            DateTime? from,
            DateTime? to,
            CancellationToken cancellationToken = default);
    }
}
