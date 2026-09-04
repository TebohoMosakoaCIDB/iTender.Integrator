using iTender.Integrator.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace iTender.Integrator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OcdsController : ControllerBase
    {
        private readonly IOcdsApiClient _ocdsApiClient;

        public OcdsController(IOcdsApiClient ocdsApiClient)
        {
            _ocdsApiClient = ocdsApiClient;
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
    }
}
