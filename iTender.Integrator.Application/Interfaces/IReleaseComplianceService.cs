using iTender.Integrator.Application.DTOs.Compliance;
using iTender.Integrator.Application.DTOs.Ocds;

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
    }
}
