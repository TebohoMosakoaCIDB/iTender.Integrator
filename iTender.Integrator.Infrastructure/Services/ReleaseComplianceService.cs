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
        private readonly IMetroDistrictRepository _metroDistrictRepository;
        private readonly IClassOfWorkTypeRepository _classOfWorkTypeRepository;
        private readonly IReleaseRepository _releaseRepository;
        private readonly ILogger<ReleaseComplianceService> _logger;

        public ReleaseComplianceService(
            IContractorComplianceService contractorComplianceService,
            ITenderRepository tenderRepository,
            IProvinceRepository provinceRepository,
            IMetroDistrictRepository metroDistrictRepository,
            IClassOfWorkTypeRepository classOfWorkTypeRepository,
            IReleaseRepository releaseRepository,
            ILogger<ReleaseComplianceService> logger)
        {
            _contractorComplianceService = contractorComplianceService;
            _tenderRepository = tenderRepository;
            _provinceRepository = provinceRepository;
            _metroDistrictRepository = metroDistrictRepository;
            _classOfWorkTypeRepository = classOfWorkTypeRepository;
            _releaseRepository = releaseRepository;
            _logger = logger;
        }

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

        private async Task<Guid?> ResolveMetroDistrictIdAsync(string? deliveryLocation, Guid? provinceId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(deliveryLocation) || provinceId is null)
                return null;

            try
            {
                var candidates = await _metroDistrictRepository.GetByProvinceAsync(provinceId.Value, cancellationToken);

                var match = candidates.FirstOrDefault(m =>
                    !string.IsNullOrWhiteSpace(m.Name) &&
                    deliveryLocation.Contains(m.Name, StringComparison.OrdinalIgnoreCase));

                if (match is not null)
                {
                    _logger.LogInformation(
                        "Best-effort metro/district match: '{MetroName}' found in deliveryLocation '{DeliveryLocation}'. Treat as a suggestion, not a certainty.",
                        match.Name, deliveryLocation);
                }

                return match?.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Metro/district lookup failed for '{DeliveryLocation}'.", deliveryLocation);
                return null;
            }
        }

        private async Task<Guid?> ResolveClassOfWorkTypeIdAsync(string? category, string? description, CancellationToken cancellationToken)
        {
            var searchText = string.Join(
                " ", new[] { category, description }.Where(s => !string.IsNullOrWhiteSpace(s)));

            if (string.IsNullOrWhiteSpace(searchText))
                return null;

            try
            {
                var candidates = await _classOfWorkTypeRepository.GetAllAsync(cancellationToken);

                var match = candidates.FirstOrDefault(c =>
                    !string.IsNullOrWhiteSpace(c.Name) &&
                    searchText.Contains(c.Name, StringComparison.OrdinalIgnoreCase));

                if (match is not null)
                {
                    _logger.LogInformation(
                        "Best-effort class-of-work match: '{ClassOfWorkName}' found in tender text '{SearchText}'. " +
                        "Unverified heuristic - spot-check before trusting.",
                        match.Name, searchText);
                }

                return match?.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Class-of-work lookup failed for '{SearchText}'.", searchText);
                return null;
            }
        }

        private static ReleaseComplianceView BuildViewFromPersisted(Domain.Entities.Release existing)
        {
            var partyViews = existing.GetContractorCandidateParties()
                .Select(p => new PartyComplianceView(
                    p.ExternalId,
                    p.Name,
                    p.RegistrationScheme,
                    p.RegistrationNumber,
                    p.ComplianceStatus,
                    "Loaded from a previous run - not re-checked against CSD/CRM."))
                .ToList();

            return new ReleaseComplianceView(
                existing.Ocid,
                existing.ReleaseId,
                existing.Tender?.ExternalId,
                existing.Tender?.Title,
                existing.Tender?.Status.ToString(),
                partyViews,
                PublishedToCrm: existing.LastSyncedAtUtc.HasValue,
                CrmTenderId: null,
                PublishReason: existing.LastSyncedAtUtc.HasValue
                    ? "Already processed in a previous run (loaded from persistence, not re-checked)."
                    : "Previously ingested but not marked synced (a prior attempt may have failed) - not re-checked this run.");
        }

        public async Task<ReleaseComplianceView> EnrichAsync(OcdsReleaseDto releaseDto, CancellationToken cancellationToken = default)
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

            var existing = await _releaseRepository.GetByOcidAndReleaseIdAsync(
                release.Ocid, release.ReleaseId, cancellationToken);

            if (existing is not null)
            {
                _logger.LogInformation(
                    "Release {Ocid}/{ReleaseId} already persisted - replaying stored result, not re-checking.",
                    release.Ocid, release.ReleaseId);
                return BuildViewFromPersisted(existing);
            }

            if (release.Tender is not null && !release.Tender.IsConstructionRelated)
            {
                _logger.LogInformation(
                    "Skipping release {Ocid}/{ReleaseId} - not construction-related (mainProcurementCategory='{Category}').",
                    release.Ocid, release.ReleaseId, release.Tender.MainProcurementCategory);

                return await PersistAndReturnAsync(
                    release,
                    Array.Empty<PartyComplianceView>(),
                    publishedToCrm: false,
                    crmTenderId: null,
                    publishReason: $"Skipped - not a construction tender " +
                        $"(mainProcurementCategory='{release.Tender.MainProcurementCategory}'). " +
                        "No CSD/CRM compliance checks were run and nothing was published.",
                    synced: true,
                    cancellationToken);
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
            var synced = false;
            Guid? crmTenderId = null;
            string? publishReason;

            if (release.Tender is null)
            {
                publishReason = "Release has no tender attached (e.g. a planning-stage release) - nothing to publish.";
                synced = true; // nothing further to do for this release
            }
            else
            {
                try
                {
                    var provinceId = await ResolveProvinceIdAsync(release.Tender.Province, cancellationToken);
                    var metroDistrictId = await ResolveMetroDistrictIdAsync(
                        release.Tender.DeliveryLocation, provinceId, cancellationToken);
                    var classOfWorkTypeId = await ResolveClassOfWorkTypeIdAsync(
                        release.Tender.Category, release.Tender.Description, cancellationToken);
                    var createModel = TenderMapper.ToCreateTenderModel(
                        release, provinceId, metroDistrictId, classOfWorkTypeId);
                    crmTenderId = await _tenderRepository.UpsertAsync(createModel, cancellationToken);
                    publishedToCrm = true;
                    synced = true;
                    publishReason = "Published to CRM.";
                }
                catch (Exception ex)
                {
                    // Same isolation principle as the per-party checks above: a CRM
                    // write failure shouldn't discard the compliance results we
                    // already have for this release. synced stays false so
                    // GetUnsyncedAsync can surface this release for a retry later -
                    // nothing currently calls GetUnsyncedAsync yet, but the release is
                    // at least persisted with a correct, honest state rather than
                    // silently reporting success.
                    _logger.LogError(
                        ex,
                        "Failed to publish tender for release {Ocid}/{ReleaseId} to CRM.",
                        release.Ocid, release.ReleaseId);

                    publishReason = $"CRM publish failed: {ex.Message}";
                }
            }

            return await PersistAndReturnAsync(
                release, partyViews, publishedToCrm, crmTenderId, publishReason, synced, cancellationToken);
        }

        private async Task<ReleaseComplianceView> PersistAndReturnAsync(Domain.Entities.Release release, IReadOnlyCollection<PartyComplianceView> partyViews, bool publishedToCrm, Guid? crmTenderId, string? publishReason, bool synced, CancellationToken cancellationToken)
        {
            if (synced)
                release.MarkSynced();

            try
            {
                await _releaseRepository.AddAsync(release, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to persist release {Ocid}/{ReleaseId} - the release was still processed this run, " +
                    "but will be re-pulled and re-processed next run since it wasn't saved.",
                    release.Ocid, release.ReleaseId);
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

        public async Task<IReadOnlyCollection<ReleaseComplianceView>> EnrichAsync(IEnumerable<OcdsReleaseDto> releaseDtos, CancellationToken cancellationToken = default)
        {
            var views = new List<ReleaseComplianceView>();

            foreach (var dto in releaseDtos)
                views.Add(await EnrichAsync(dto, cancellationToken));

            return views;
        }
    }
}