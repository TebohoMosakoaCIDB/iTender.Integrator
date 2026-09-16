using iTender.Integrator.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace iTender.Integrator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractorsController : ControllerBase
    {
        private readonly IContractorProfileService _contractorProfileService;

        public ContractorsController(IContractorProfileService contractorProfileService)
        {
            _contractorProfileService = contractorProfileService;
        }

        // Full contractor profile - CRM (grading, sanctions, moratorium) joined to
        // CSD (registration, tax, accreditation, directors, BEE) via the CSD number
        // found on the CRM record. Returns 200 even if one or both sides couldn't
        // be resolved - CrmLookupReason/CsdLookupReason in the body explain why
        // rather than the caller getting an opaque 404/500 for a partial result.
        [HttpGet("{crsNumber}")]
        public async Task<IActionResult> GetByCrsNumber(string crsNumber, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(crsNumber))
                return BadRequest("CRS number is required.");

            var profile = await _contractorProfileService.GetByCrsNumberAsync(crsNumber, cancellationToken);

            return Ok(profile);
        }
    }
}
