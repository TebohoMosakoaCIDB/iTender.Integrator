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

        // Deliberately a separate, explicit call - never triggered automatically
        // by tender creation/publishing. Until a real gateway is wired in
        // (INotificationSender), every outcome will report Sent=false with a
        // "logged only" reason - the value right now is the matching itself
        // (who WOULD be notified), which the response is honest about.
        [HttpPost("qualified-contractors")]
        public async Task<IActionResult> NotifyQualifiedContractors(
            [FromBody] NotifyQualifiedContractorsRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _notificationService.NotifyQualifiedContractorsAsync(
                request.ProvinceId,
                request.ClassOfWorkTypeId,
                request.RequiredGradingDesignationContains,
                request.Subject,
                request.Message,
                cancellationToken);

            return Ok(result);
        }
    }
}
