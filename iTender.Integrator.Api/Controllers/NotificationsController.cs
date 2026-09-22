using iTender.Integrator.Application.DTOs.Compliance;
using iTender.Integrator.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace iTender.Integrator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IContractorNotificationService _notificationService;

        public NotificationsController(IContractorNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("qualified-contractors")]
        public async Task<IActionResult> NotifyQualifiedContractors(
            [FromBody] NotifyQualifiedContractorsRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _notificationService.NotifyQualifiedContractorsAsync(
                request.ProvinceId,
                request.RequiredGradingDesignationContains,
                request.Subject,
                request.Message,
                cancellationToken);

            return Ok(result);
        }
    }
}
