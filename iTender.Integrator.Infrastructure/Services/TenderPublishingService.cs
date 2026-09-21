using iTender.Integrator.Application.DTOs.eTenders;
using iTender.Integrator.Application.DTOs.ETenders;
using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Infrastructure.Mappers.Crm;
using Microsoft.Extensions.Logging;

namespace iTender.Integrator.Infrastructure.Services
{
    public sealed class TenderPublishingService : ITenderPublishingService
    {
        private readonly IETendersAdminApiClient _etendersClient;
        private readonly ITenderRepository _tenderRepository;
        private readonly ILogger<TenderPublishingService> _logger;

        public TenderPublishingService(
            IETendersAdminApiClient etendersClient,
            ITenderRepository tenderRepository,
            ILogger<TenderPublishingService> logger)
        {
            _etendersClient = etendersClient;
            _tenderRepository = tenderRepository;
            _logger = logger;
        }

        public async Task<CreateTenderResult> CreateTenderAsync(
            CreateTenderRequest request,
            IReadOnlyCollection<TenderDocumentUpload>? documents = null,
            CancellationToken cancellationToken = default)
        {
            var etendersResponse = await _etendersClient.CreateTenderAsync(request, documents, cancellationToken);

            if (!etendersResponse.Success)
            {
                _logger.LogWarning(
                    "eTenders rejected tender creation for '{TenderNumber}': {Message}",
                    request.TenderNumber, etendersResponse.Message);

                return new CreateTenderResult(
                    EtendersSuccess: false,
                    etendersResponse.Message,
                    EtendersTenderId: null,
                    EtendersTenderNumber: null,
                    PublishedToCrm: false,
                    CrmTenderId: null,
                    CrmPublishReason: "Skipped - eTenders creation failed, nothing to publish to CRM.");
            }

            var publishedToCrm = false;
            Guid? crmTenderId = null;
            string crmReason;

            if (!request.IsConstructionTender)
            {
                crmReason = "Skipped - not flagged as a construction tender.";
            }
            else
            {
                try
                {
                    var createModel = TenderMapper.FromETendersCreateRequest(request, etendersResponse.Data);
                    crmTenderId = await _tenderRepository.UpsertAsync(createModel, cancellationToken);
                    publishedToCrm = true;
                    crmReason = "Published to CRM.";
                }
                catch (Exception ex)
                {
                    // Same isolation principle used everywhere else in this
                    // pipeline: the eTenders tender already exists at this point -
                    // a CRM failure shouldn't be reported as if tender creation
                    // itself failed.
                    _logger.LogError(
                        ex, "Failed to publish tender '{TenderNumber}' to CRM after successful eTenders creation.",
                        request.TenderNumber);

                    crmReason = $"CRM publish failed: {ex.Message}";
                }
            }

            return new CreateTenderResult(
                EtendersSuccess: true,
                etendersResponse.Message,
                etendersResponse.Data?.Id,
                etendersResponse.Data?.TenderNo,
                publishedToCrm,
                crmTenderId,
                crmReason);
        }
    }
}
