using iTender.Integrator.Application.DTOs.ETenders;
using iTender.Integrator.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iTender.Integrator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TendersController : ControllerBase
    {
        private readonly ITenderPublishingService _tenderPublishingService;

        public TendersController(ITenderPublishingService tenderPublishingService)
        {
            _tenderPublishingService = tenderPublishingService;
        }

        // multipart/form-data, matching eTenders' own /api/Etenders/add shape plus
        // our own added IsConstructionTender flag. Creates the tender in eTenders
        // first; if IsConstructionTender=true and that succeeds, also publishes it
        // to CRM. See ITenderPublishingService for how a partial outcome (eTenders
        // ok, CRM failed) is represented rather than hidden.
        [HttpPost]
        public async Task<IActionResult> CreateTender(
            [FromForm] CreateTenderRequest request,
            [FromForm(Name = "Documents")] List<IFormFile>? documents,
            CancellationToken cancellationToken)
        {
            var uploads = documents?
                .Where(f => f.Length > 0)
                .Select(f => new TenderDocumentUpload(f.FileName, f.ContentType, f.OpenReadStream()))
                .ToList();

            var result = await _tenderPublishingService.CreateTenderAsync(request, uploads, cancellationToken);

            return Ok(result);
        }
    }
}
