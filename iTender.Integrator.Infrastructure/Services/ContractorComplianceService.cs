using iTender.Integrator.Application.DTOs.Compliance;
using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Domain.Entities;
using iTender.Integrator.Domain.Entities.Csd;
using iTender.Integrator.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace iTender.Integrator.Infrastructure.Services
{
    public sealed class ContractorComplianceService : IContractorComplianceService
    {
        private readonly ICsdApiClient _csdApiClient;
        private readonly IContractorGradeRepository _contractorGradeRepository;
        private readonly ILogger<ContractorComplianceService> _logger;

        public ContractorComplianceService(
            ICsdApiClient csdApiClient,
            IContractorGradeRepository contractorGradeRepository,
            ILogger<ContractorComplianceService> logger)
        {
            _csdApiClient = csdApiClient;
            _contractorGradeRepository = contractorGradeRepository;
            _logger = logger;
        }

        public async Task<Party> CheckAndApplyAsync(
            Party party, CancellationToken cancellationToken = default)
        {
            var result = await CheckAsync(party, cancellationToken);
            party.SetComplianceStatus(result.Status);
            return party;
        }

        public async Task<ContractorComplianceResult> CheckAsync(
            Party party, CancellationToken cancellationToken = default)
        {
            var checkedAtUtc = DateTime.UtcNow;

            if (!party.HasRegistrationIdentifier)
            {
                return Result(
                    party,
                    null,
                    CidbComplianceStatus.RegistrationNotFound,
                    "Party carries no registration identifier (OCDS identifier.id was empty) - nothing to look up in CSD or CRM.",
                    csdFound: false,
                    crmFound: false,
                    checkedAtUtc);
            }

            var registrationNumber = party.RegistrationNumber!.Trim();

            // 1) CSD: is this a legitimately registered, active supplier at all?
            CsdSupplier? supplier;
            try
            {
                supplier = await _csdApiClient.GetSupplierDetailsAsync(registrationNumber, cancellationToken);
            }
            catch (Exception ex)
            {
                // CSD treats "supplier not found" as a business error (400/401) rather
                // than a distinct not-found response - GetSupplierDetailsAsync surfaces
                // that as an exception today. We can't yet tell "not found" apart from
                // "CSD is down" from here; both fall back to RegistrationNotFound so a
                // compliance decision is never made on a failed lookup.
                _logger.LogWarning(
                    ex,
                    "CSD lookup failed for registration number {RegistrationNumber} on party {PartyExternalId}.",
                    registrationNumber, party.ExternalId);

                return Result(
                    party,
                    null,
                    CidbComplianceStatus.RegistrationNotFound,
                    $"CSD lookup failed for '{registrationNumber}': {ex.Message}",
                    csdFound: false,
                    crmFound: false,
                    checkedAtUtc);
            }

            if (supplier.SupplierIdentification is { IsActive: false })
            {
                return Result(
                    party,
                    null,
                    CidbComplianceStatus.RegistrationSuspended,
                    $"CSD reports supplier '{registrationNumber}' as inactive" +
                        (string.IsNullOrWhiteSpace(supplier.SupplierIdentification.SupplierInactiveReason)
                            ? "."
                            : $" ({supplier.SupplierIdentification.SupplierInactiveReason})."),
                    csdFound: true,
                    crmFound: false,
                    checkedAtUtc);
            }

            if (supplier.Tax is { IsRegistered: false })
            {
                return Result(
                    party,
                    null,
                    CidbComplianceStatus.NonCompliant,
                    $"CSD reports supplier '{registrationNumber}' as not tax registered.",
                    csdFound: true,
                    crmFound: false,
                    checkedAtUtc);
            }

            // 2) CRM: the actual CIDB grading and sanction/moratorium record, keyed by
            // the same CSD number. CSD confirms the supplier exists and is active; it
            // does not carry a class of work or grade at all.
            var contractor = await _contractorGradeRepository.GetByCsdNumberAsync(registrationNumber, cancellationToken);

            if (contractor is null)
            {
                return Result(
                    party,
                    null,
                    CidbComplianceStatus.RegistrationNotFound,
                    $"Supplier '{registrationNumber}' is active on CSD but has no matching CIDB contractor record on iTender.",
                    csdFound: true,
                    crmFound: false,
                    checkedAtUtc);
            }

            if (contractor.IsSanctioned)
            {
                return Result(
                    party,
                    contractor.CrsNumber,
                    CidbComplianceStatus.NonCompliant,
                    $"Contractor '{registrationNumber}' is currently sanctioned on iTender.",
                    csdFound: true,
                    crmFound: true,
                    checkedAtUtc);
            }

            if (contractor.IsMoratorium)
            {
                return Result(
                    party,
                    contractor.CrsNumber,
                    CidbComplianceStatus.RegistrationSuspended,
                    $"Contractor '{registrationNumber}' is under a CIDB moratorium on iTender.",
                    csdFound: true,
                    crmFound: true,
                    checkedAtUtc);
            }

            if (string.IsNullOrWhiteSpace(contractor.CurrentContractorGradingDesignation))
            {
                return Result(
                    party, 
                    contractor.CrsNumber,
                    CidbComplianceStatus.GradingInsufficient,
                    $"Contractor '{registrationNumber}' has no current grading designation on file on iTender.",
                    csdFound: true,
                    crmFound: true,
                    checkedAtUtc);
            }

            // NOTE: we are not yet checking whether the grading designation actually
            // covers the tender's class of work / value range, or whether it has
            // expired (ContractorModel doesn't carry a grading expiry date the way
            // Contractor.CidbGrading does in the domain model). That's the next
            // refinement once we know the real shape/format of
            // CurrentContractorGradingDesignation (e.g. "8GB PE") from a live CRM record.
            return Result(
                party,
                contractor.CrsNumber,
                CidbComplianceStatus.Compliant,
                $"Contractor '{registrationNumber}' is active on CSD, not sanctioned or under moratorium, " +
                    $"and holds grading designation '{contractor.CurrentContractorGradingDesignation}'.",
                csdFound: true,
                crmFound: true,
                checkedAtUtc);
        }

        private static ContractorComplianceResult Result(
            Party party,
            string? CRSNumber,
            CidbComplianceStatus status,
            string reason,
            bool csdFound,
            bool crmFound,
            DateTime checkedAtUtc)
            => new(
                party.ExternalId,
                CRSNumber,
                party.RegistrationNumber,
                status,
                status.ToString(),
                reason,
                csdFound,
                crmFound,
                checkedAtUtc);
    }
}
