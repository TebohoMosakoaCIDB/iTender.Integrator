using iTender.Integrator.Application.DTOs.Compliance;
using iTender.Integrator.Application.DTOs.Ocds;
using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Application.Mapping;
using iTender.Integrator.Domain.Entities;
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

        private sealed record ProcessOutcome(
            IReadOnlyCollection<PartyComplianceView> PartyViews,
            bool PublishedToCrm,
            Guid? CrmTenderId,
            string? PublishReason,
            bool Synced);

        // Everything EnrichAsync and RetryAsync share: construction filter, party
        // compliance checks, and the CRM publish attempt. Deliberately takes a
        // fully-formed Release rather than a DTO - EnrichAsync gets there by mapping
        // a fresh OCDS payload, RetryAsync gets there by loading one back out of
        // IReleaseRepository. Persistence (Add vs Update) is the one thing that
        // differs between the two callers, so it stays out of this method.
        private async Task<ProcessOutcome> ProcessReleaseAsync(
            Domain.Entities.Release release, CancellationToken cancellationToken)
        {
            if (release.Tender is not null && !release.Tender.IsConstructionRelated)
            {
                _logger.LogInformation(
                    "Skipping release {Ocid}/{ReleaseId} - not construction-related (mainProcurementCategory='{Category}').",
                    release.Ocid, release.ReleaseId, release.Tender.MainProcurementCategory);

                return new ProcessOutcome(
                    Array.Empty<PartyComplianceView>(),
                    PublishedToCrm: false,
                    CrmTenderId: null,
                    PublishReason: $"Skipped - not a construction tender " +
                        $"(mainProcurementCategory='{release.Tender.MainProcurementCategory}'). " +
                        "No CSD/CRM compliance checks were run and nothing was published.",
                    Synced: true);
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
                    _logger.LogError(
                        ex,
                        "Failed to publish tender for release {Ocid}/{ReleaseId} to CRM.",
                        release.Ocid, release.ReleaseId);

                    publishReason = $"CRM publish failed: {ex.Message}";
                }
            }

            return new ProcessOutcome(partyViews, publishedToCrm, crmTenderId, publishReason, synced);
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

            var outcome = await ProcessReleaseAsync(release, cancellationToken);

            return await PersistAndReturnAsync(release, outcome, isNew: true, cancellationToken);
        }

        private ReleaseComplianceView BuildViewFromPersisted(Release existing)
        {
            throw new NotImplementedException();
        }

        public async Task<ReleaseComplianceView> RetryAsync(
            Domain.Entities.Release release, CancellationToken cancellationToken = default)
        {
            if (release is null) throw new ArgumentNullException(nameof(release));

            _logger.LogInformation(
                "Retrying previously unsynced release {Ocid}/{ReleaseId}.", release.Ocid, release.ReleaseId);

            var outcome = await ProcessReleaseAsync(release, cancellationToken);

            return await PersistAndReturnAsync(release, outcome, isNew: false, cancellationToken);
        }

        public async Task<IReadOnlyCollection<ReleaseComplianceView>> RetryUnsyncedAsync(
            int take = 100, CancellationToken cancellationToken = default)
        {
            var unsynced = await _releaseRepository.GetUnsyncedAsync(take, cancellationToken);

            if (unsynced.Count == 0)
                return Array.Empty<ReleaseComplianceView>();

            _logger.LogInformation("Retrying {Count} previously unsynced release(s).", unsynced.Count);

            var views = new List<ReleaseComplianceView>(unsynced.Count);

            foreach (var release in unsynced)
                views.Add(await RetryAsync(release, cancellationToken));

            return views;
        }

        private async Task<ReleaseComplianceView> PersistAndReturnAsync(
            Domain.Entities.Release release,
            ProcessOutcome outcome,
            bool isNew,
            CancellationToken cancellationToken)
        {
            if (outcome.Synced)
                release.MarkSynced();

            try
            {
                if (isNew)
                    await _releaseRepository.AddAsync(release, cancellationToken);
                else
                    await _releaseRepository.UpdateAsync(release, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to persist release {Ocid}/{ReleaseId} ({Operation}) - the release was still " +
                    "processed this run, but the outcome wasn't saved.",
                    release.Ocid, release.ReleaseId, isNew ? "insert" : "update");
            }

            return new ReleaseComplianceView(
                release.Ocid,
                release.ReleaseId,
                release.Tender?.ExternalId,
                release.Tender?.Title,
                release.Tender?.Status.ToString(),
                outcome.PartyViews,
                outcome.PublishedToCrm,
                outcome.CrmTenderId,
                outcome.PublishReason);
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