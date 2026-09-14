using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Domain.Entities;
using iTender.Integrator.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iTender.Integrator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplianceController : ControllerBase
    {
        private readonly IContractorComplianceService _complianceService;

        public ComplianceController(IContractorComplianceService complianceService)
        {
            _complianceService = complianceService;
        }

        // Ad-hoc check by registration number, independent of a real OCDS release -
        // useful for testing the CSD <-> CRM join on its own before it's wired into
        // the release-ingestion pipeline. registrationScheme defaults to ZA-CSD but
        // isn't currently used for anything beyond the resulting Party record, since
        // both CSD and CRM are keyed by the CSD number regardless of scheme label.
        [HttpGet("suppliers/{MAAANumber}")]
        public async Task<IActionResult> CheckSupplier(
            string MAAANumber,
            [FromQuery] string registrationScheme = "ZA-CSD",
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(MAAANumber))
                return BadRequest("Registration number is required.");

            var party = Party.Create(
                externalId: $"adhoc-{MAAANumber}",
                name: MAAANumber,
                roles: PartyRole.Supplier,
                registrationScheme: registrationScheme,
                registrationNumber: MAAANumber);

            var result = await _complianceService.CheckAsync(party, cancellationToken);

            return Ok(result);
        }
    }
}
