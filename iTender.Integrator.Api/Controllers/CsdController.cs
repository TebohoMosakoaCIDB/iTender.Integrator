using iTender.Integrator.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iTender.Integrator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CsdController : ControllerBase
    {
        private readonly ICsdApiClient _csdApiClient;

        public CsdController(ICsdApiClient csdApiClient)
        {
            _csdApiClient = csdApiClient;
        }

        [HttpGet("suppliers/{MAAANumber}")]
        public async Task<IActionResult> GetSupplier(
            string supplierNumber,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(supplierNumber))
            {
                return BadRequest("Supplier number is required.");
            }

            try
            {
                var supplier = await _csdApiClient.GetSupplierDetailsAsync(
                    supplierNumber,
                    cancellationToken);

                return Ok(supplier);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(
                    StatusCodes.Status502BadGateway,
                    new
                    {
                        message = "Unable to retrieve supplier information from CSD.",
                        detail = ex.Message
                    });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
