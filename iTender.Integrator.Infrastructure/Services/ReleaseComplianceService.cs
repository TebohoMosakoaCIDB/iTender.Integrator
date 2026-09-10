using iTender.Integrator.Application.DTOs.Compliance;
using iTender.Integrator.Application.DTOs.Ocds;
using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Application.Mapping;
using iTender.Integrator.Infrastructure.Mappers.Crm;
using Microsoft.Extensions.Logging;

namespace iTender.Integrator.Infrastructure.Services
{
    public sealed class ReleaseComplianceService : IReleaseComplianceService
    {
        private readonly IContractorComplianceService _contractorComplianceService;
        private readonly ITenderRepository _tenderRepository;
        private readonly IProvinceRepository _provinceRepository;
        private readonly ILogger<ReleaseComplianceService> _logger;

        public ReleaseComplianceService(
            IContractorComplianceService contractorComplianceService,
            ITenderRepository tenderRepository,
            IProvinceRepository provinceRepository,
            ILogger<ReleaseComplianceService> logger)
        {
            _contractorComplianceService = contractorComplianceService;
            _tenderRepository = tenderRepository;
            _provinceRepository = provinceRepository;
            _logger = logger;
        }

        // "National" is not a real province - per instruction, it (and any name that
        // doesn't match a CRM province record) resolves to null rather than guessed
        // at. A failed CRM lookup here also falls back to null rather than failing
        // the whole publish - a missing province shouldn't block the tender existing.
        private async Task<Guid?> ResolveProvinceIdAsync(string? provinceName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(provinceName))
                return null;

            if (string.Equals(provinceName.Trim(), "National", StringComparison.OrdinalIgnoreCase))
                return null;

            try
            {
                var province = await _provinceRepository.GetByNameAsync(provinceName, cancellationToken);

                if (province is null)
                {
                    _logger.LogWarning(
                        "No CRM province record matched OCDS province '{ProvinceName}'.", provinceName);
                }

                return province?.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Province lookup failed for '{ProvinceName}'.", provinceName);
                return null;
            }
        }

        public async Task<ReleaseComplianceView> EnrichAsync(
            OcdsReleaseDto releaseDto, CancellationToken cancellationToken = default)
        {
            if (releaseDto is null) throw new ArgumentNullException(nameof(releaseDto));

            Domain.Entities.Release release;
            try
            {
                release = OcdsReleaseMapper.ToDomain(releaseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to map release {OcId}/{Id} to the domain model.",
                    releaseDto.OcId, releaseDto.Id);

                return new ReleaseComplianceView(
                    releaseDto.OcId ?? string.Empty,
                    releaseDto.Id ?? string.Empty,
                    null,
                    null,
                    null,
                    Array.Empty<PartyComplianceView>(),
                    PublishedToCrm: false,
                    CrmTenderId: null,
                    PublishReason: "Release could not be mapped to the domain model - see logs.");
            }

            var candidates = release.GetContractorCandidateParties().ToList();
            var partyViews = new List<PartyComplianceView>(candidates.Count);

            foreach (var party in candidates)
            {
                // Sequential on purpose: each check is a CSD auth + supplier lookup
                // plus a CRM query. Running these concurrently risks hammering CSD's
                // token endpoint with parallel Authenticate calls from a single
                // client instance. Fine for a first pass; worth revisiting (e.g. a
                // shared cached token, then bounded concurrency) if a release ever
                // carries many parties.
                try
                {
                    var result = await _contractorComplianceService.CheckAsync(party, cancellationToken);
                    party.SetComplianceStatus(result.Status);

                    partyViews.Add(new PartyComplianceView(
                        party.ExternalId,
                        party.Name,
                        party.RegistrationScheme,
                        party.RegistrationNumber,
                        result.Status,
                        result.Reason));
                }
                catch (Exception ex)
                {
                    // A single party failing (CSD/CRM outage, unexpected data) should
                    // not take down the whole release view - log it and surface it as
                    // an explicit unchecked result rather than losing the party.
                    _logger.LogError(
                        ex,
                        "Compliance check threw for party {PartyExternalId} on release {Ocid}/{ReleaseId}.",
                        party.ExternalId, release.Ocid, release.ReleaseId);

                    partyViews.Add(new PartyComplianceView(
                        party.ExternalId,
                        party.Name,
                        party.RegistrationScheme,
                        party.RegistrationNumber,
                        Domain.Enums.CidbComplianceStatus.NotChecked,
                        $"Compliance check failed unexpectedly: {ex.Message}"));
                }
            }

            var publishedToCrm = false;
            Guid? crmTenderId = null;
            string? publishReason;

            if (release.Tender is null)
            {
                publishReason = "Release has no tender attached (e.g. a planning-stage release) - nothing to publish.";
            }
            else
            {
                try
                {
                    var provinceId = await ResolveProvinceIdAsync(release.Tender.Province, cancellationToken);
                    var createModel = TenderMapper.ToCreateTenderModel(release, provinceId);
                    crmTenderId = await _tenderRepository.UpsertAsync(createModel, cancellationToken);
                    publishedToCrm = true;
                    publishReason = "Published to CRM.";
                }
                catch (Exception ex)
                {
                    // Same isolation principle as the per-party checks above: a CRM
                    // write failure shouldn't discard the compliance results we
                    // already have for this release.
                    _logger.LogError(
                        ex,
                        "Failed to publish tender for release {Ocid}/{ReleaseId} to CRM.",
                        release.Ocid, release.ReleaseId);

                    publishReason = $"CRM publish failed: {ex.Message}";
                }
            }

            return new ReleaseComplianceView(
                release.Ocid,
                release.ReleaseId,
                release.Tender?.ExternalId,
                release.Tender?.Title,
                release.Tender?.Status.ToString(),
                partyViews,
                publishedToCrm,
                crmTenderId,
                publishReason);
        }

        public async Task<IReadOnlyCollection<ReleaseComplianceView>> EnrichAsync(
            IEnumerable<OcdsReleaseDto> releaseDtos, CancellationToken cancellationToken = default)
        {
            var views = new List<ReleaseComplianceView>();

            foreach (var dto in releaseDtos)
                views.Add(await EnrichAsync(dto, cancellationToken));

            return views;
        }
    }
}