using iTender.Integrator.Application.Interfaces;
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

            // Was: `var profile = _trackRecordRepository.GetByCrsNumberAsync(crsNumber);`
            // - missing await, so `profile` held a Task<T> (always non-null,
            // "success" looking) rather than the actual result, and the
            // cancellationToken parameter was never passed through at all.
            var records = await _trackRecordRepository.GetByCrsNumberAsync(crsNumber, cancellationToken);

            return Ok(records);
        }
    }
}
