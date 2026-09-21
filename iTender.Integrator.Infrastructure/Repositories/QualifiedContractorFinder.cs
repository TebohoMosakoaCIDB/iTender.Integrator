using iTender.Integrator.Application.DTOs.Crm;
using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Domain.Constants;
using iTender.Integrator.Domain.Enums;
using iTender.Integrator.Infrastructure.Integrations.CRM;
using iTender.Integrator.Infrastructure.Mappers.Crm;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk.Query;
using static iTender.Integrator.Domain.Constants.CrmFieldNames;

namespace iTender.Integrator.Infrastructure.Repositories
{
    public class QualifiedContractorFinder : IQualifiedContractorFinder
    {
        private readonly ICrmServiceFactory _crmServiceFactory;
        private readonly ILogger<QualifiedContractorFinder> _logger;

        public QualifiedContractorFinder(
            ICrmServiceFactory crmServiceFactory, ILogger<QualifiedContractorFinder> logger)
        {
            _crmServiceFactory = crmServiceFactory;
            _logger = logger;
        }

        public Task<IReadOnlyCollection<ContractorModel>> FindQualifiedContractorsAsync(
            Guid? provinceId,
            string? requiredGradingDesignationContains,
            CancellationToken cancellationToken = default)
        {
            var query = new QueryExpression(CrmEntityNames.Account)
            {
                ColumnSet = new ColumnSet(
                    ContractorFields.Id,
                    ContractorFields.Name,
                    ContractorFields.TradingAs,
                    ContractorFields.CrsNumber,
                    ContractorFields.CSDNumber,
                    ContractorFields.ProvinceId,
                    ContractorFields.CurrentContractorGradingDesignation,
                    ContractorFields.CurrentContractorGrade,
                    ContractorFields.Phone,
                    ContractorFields.Email,
                    ContractorFields.IsSanctioned,
                    ContractorFields.Moratorium,
                    ContractorFields.StatusCode)
            };

            // Confirmed contractor statuscode (iTenderContractorStatus) - only truly
            // Active contractors are eligible. Sanctioned/moratorium are separate
            // flags checked below, not folded into statuscode.
            query.Criteria.AddCondition(
                ContractorFields.StatusCode, ConditionOperator.Equal, (int)iTenderContractorStatus.Active);

            query.Criteria.AddCondition(ContractorFields.IsSanctioned, ConditionOperator.Equal, false);
            query.Criteria.AddCondition(ContractorFields.Moratorium, ConditionOperator.Equal, false);

            // Province IS a real lookup on both sides - exact match, no ambiguity.
            if (provinceId.HasValue)
                query.Criteria.AddCondition(ContractorFields.ProvinceId, ConditionOperator.Equal, provinceId.Value);

            var service = _crmServiceFactory.Create();
            var result = service.RetrieveMultiple(query);

            var candidates = result.Entities
                .Select(ContractorMapper.ToDomain)
                .Where(c => c is not null)
                .Select(c => c!)
                .ToList();

            if (string.IsNullOrWhiteSpace(requiredGradingDesignationContains))
            {
                IReadOnlyCollection<ContractorModel> all = candidates;
                return Task.FromResult(all);
            }

            // Best-effort only - see the interface doc comment. Every match (and
            // every rejection) is worth being able to audit, so log the count
            // rather than silently filtering.
            var matched = candidates
                .Where(c =>
                    (c.CurrentContractorGradingDesignation?.Contains(
                        requiredGradingDesignationContains, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.CurrentGrade?.Contains(
                        requiredGradingDesignationContains, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();

            _logger.LogInformation(
                "Qualified contractor search: {CandidateCount} active/eligible contractors in province, " +
                "{MatchedCount} matched grading text '{RequiredGrading}' (best-effort substring match - " +
                "verify format assumptions before trusting this at scale).",
                candidates.Count, matched.Count, requiredGradingDesignationContains);

            IReadOnlyCollection<ContractorModel> matchedResult = matched;
            return Task.FromResult(matchedResult);
        }
    }
}
