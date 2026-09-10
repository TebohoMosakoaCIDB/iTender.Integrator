using iTender.Integrator.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace iTender.Integrator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OcdsController : ControllerBase
    {
        private readonly IOcdsApiClient _ocdsApiClient;
        private readonly IReleaseComplianceService _releaseComplianceService;

        public OcdsController(IOcdsApiClient ocdsApiClient, IReleaseComplianceService releaseComplianceService)
        {
            _ocdsApiClient = ocdsApiClient;
            _releaseComplianceService = releaseComplianceService;
        }

        [HttpGet("releases")]
        public async Task<IActionResult> GetReleases(
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            CancellationToken cancellationToken)
        {
            var result = await _ocdsApiClient.GetReleasesAsync(
                pageNumber ?? 1,
                pageSize ?? 50,
                from,
                to,
                cancellationToken);

            return Ok(result);
        }

        // Slide 6: "Publish once. Verify automatically." - pulls releases exactly
        // like GetReleases above, but maps each one to the domain model and runs
        // the CSD/CRM compliance check against every candidate contractor/tenderer
        // party before returning. Nothing is persisted; this is a point-in-time
        // view, not a sync.
        [HttpGet("releases/compliance-checked")]
        public async Task<IActionResult> GetReleasesWithCompliance(
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            CancellationToken cancellationToken)
        {
            var package = await _ocdsApiClient.GetReleasesAsync(
                pageNumber ?? 1,
                pageSize ?? 50,
                from,
                to,
                cancellationToken);

            var views = await _releaseComplianceService.EnrichAsync(package.Releases, cancellationToken);

            return Ok(views);
        }
    }
}
