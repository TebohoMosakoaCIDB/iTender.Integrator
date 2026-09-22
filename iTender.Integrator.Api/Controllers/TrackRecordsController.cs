using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace iTender.Integrator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackRecordsController : ControllerBase
    {
        private readonly ITrackRecordRepository _trackRecordRepository;

        public TrackRecordsController(ITrackRecordRepository trackRecordRepository)
        {
            _trackRecordRepository = trackRecordRepository;
        }

        [HttpGet("{crsNumber}")]
        public async Task<IActionResult> GetByCrsNumber(string crsNumber, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(crsNumber))
                return BadRequest("CRS number is required.");

            var profile = _trackRecordRepository.GetByCrsNumberAsync(crsNumber);

            return Ok(profile);
        }
    }
}
