using iTender.Integrator.Application.DTOs.Compliance;
using iTender.Integrator.Application.DTOs.Ocds;
using iTender.Integrator.Domain.Entities;

namespace iTender.Integrator.Application.Interfaces
{
    public interface IReleaseComplianceService
    {
        Task<ReleaseComplianceView> EnrichAsync(
            OcdsReleaseDto releaseDto,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<ReleaseComplianceView>> EnrichAsync(
            IEnumerable<OcdsReleaseDto> releaseDtos,
            CancellationToken cancellationToken = default);

        Task<ReleaseComplianceView> RetryAsync(
            Release release,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<ReleaseComplianceView>> RetryUnsyncedAsync(
            int take = 100,
            CancellationToken cancellationToken = default);
    }
}
