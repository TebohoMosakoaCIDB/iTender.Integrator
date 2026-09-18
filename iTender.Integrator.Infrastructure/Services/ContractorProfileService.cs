using iTender.Integrator.Application.DTOs.Compliance;
using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Domain.Entities.Csd;
using Microsoft.Extensions.Logging;

namespace iTender.Integrator.Infrastructure.Services
{
    public sealed class ContractorProfileService : IContractorProfileService
    {
        private readonly IContractorGradeRepository _contractorGradeRepository;
        private readonly IContractorRepository _contractorRepository;
        private readonly ICsdApiClient _csdApiClient;
        private readonly ILogger<ContractorProfileService> _logger;

        public ContractorProfileService(
            IContractorGradeRepository contractorGradeRepository,
            IContractorRepository contractorRepository,
            ICsdApiClient csdApiClient,
            ILogger<ContractorProfileService> logger)
        {
            _contractorGradeRepository = contractorGradeRepository;
            _contractorRepository = contractorRepository;
            _csdApiClient = csdApiClient;
            _logger = logger;
        }

        public async Task<ContractorFullProfile> GetByCrsNumberAsync(
            string crsNumber, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(crsNumber))
                throw new ArgumentException("CRS number is required.", nameof(crsNumber));

            crsNumber = crsNumber.Trim();

            Application.DTOs.Crm.ContractorModel? contractor;
            string crmReason;

            try
            {
                contractor = await _contractorRepository.GetByCrsNumberAsync(crsNumber, cancellationToken);
                crmReason = contractor is null
                    ? $"No CRM contractor record found for CRS number '{crsNumber}'."
                    : "Found.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CRM lookup failed for CRS number '{CrsNumber}'.", crsNumber);
                contractor = null;
                crmReason = $"CRM lookup failed: {ex.Message}";
            }

            if (contractor is null)
            {
                return new ContractorFullProfile(
                    crsNumber,
                    Crm: null,
                    Csd: null,
                    CrmLookupReason: crmReason,
                    CsdLookupReason: "Skipped - no CRM record to get a CSD number from.");
            }

            if (string.IsNullOrWhiteSpace(contractor.CsdNumber))
            {
                return new ContractorFullProfile(
                    crsNumber,
                    contractor,
                    Csd: null,
                    crmReason,
                    CsdLookupReason: "Skipped - CRM record has no CSD number on file.");
            }

            CsdSupplier? supplier;
            string csdReason;

            try
            {
                supplier = await _csdApiClient.GetSupplierDetailsAsync(contractor.CsdNumber, cancellationToken);
                csdReason = "Found.";
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "CSD lookup failed for CSD number '{CsdNumber}' (CRS number '{CrsNumber}').",
                    contractor.CsdNumber, crsNumber);
                supplier = null;
                csdReason = $"CSD lookup failed: {ex.Message}";
            }

            return new ContractorFullProfile(crsNumber, contractor, supplier, crmReason, csdReason);
        }
    }
}
