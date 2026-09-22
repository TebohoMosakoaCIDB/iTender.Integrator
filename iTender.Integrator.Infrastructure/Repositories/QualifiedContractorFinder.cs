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
            Guid? classOfWorkTypeId,
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
                    ContractorFields.StatusCode),
                // classOfWorkTypeId joins in a second QueryExpression for the same
                // logical account rows (inner join via nv_classofwork below can
                // return duplicate account rows if a contractor somehow has more
                // than one graded row for the same class - Distinct guards against
                // that rather than assuming the data is always clean).
                Distinct = true
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

            if (classOfWorkTypeId.HasValue)
            {
                // Precise match: only contractors with an actual nv_classofwork
                // row for this class of work type. This replaces the old
                // fuzzy-text-only approach now that ContractorGradeFields confirms
                // a real lookup exists - see the interface doc comment.
                var gradeLink = query.AddLink(
                    CrmEntityNames.ClassOfWork,
                    ContractorFields.Id,
                    ContractorGradeFields.ContractorId,
                    JoinOperator.Inner);

                gradeLink.LinkCriteria.AddCondition(
                    ContractorGradeFields.ClassOfWorkTypeId, ConditionOperator.Equal, classOfWorkTypeId.Value);
            }

            var service = _crmServiceFactory.Create();
            var result = service.RetrieveMultiple(query);

            var candidates = result.Entities
                .Select(ContractorMapper.ToDomain)
                .Where(c => c is not null)
                .Select(c => c!)
                .ToList();

            // A precise classOfWorkTypeId match already did the real filtering via
            // the join - the fuzzy text match only kicks in as a fallback when the
            // caller didn't have a resolved class-of-work id to join on.
            if (classOfWorkTypeId.HasValue || string.IsNullOrWhiteSpace(requiredGradingDesignationContains))
            {
                _logger.LogInformation(
                    "Qualified contractor search: {Count} contractors matched (province filter: {HasProvince}, " +
                    "class-of-work join: {HasClassOfWork}).",
                    candidates.Count, provinceId.HasValue, classOfWorkTypeId.HasValue);

                IReadOnlyCollection<ContractorModel> all = candidates;
                return Task.FromResult(all);
            }

            var matched = candidates
                .Where(c =>
                    (c.CurrentContractorGradingDesignation?.Contains(
                        requiredGradingDesignationContains, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.CurrentGrade?.Contains(
                        requiredGradingDesignationContains, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();

            _logger.LogInformation(
                "Qualified contractor search (fuzzy fallback - no ClassOfWorkTypeId supplied): " +
                "{CandidateCount} active/eligible contractors, {MatchedCount} matched grading text " +
                "'{RequiredGrading}'. Prefer classOfWorkTypeId when available - this is unverified.",
                candidates.Count, matched.Count, requiredGradingDesignationContains);

            IReadOnlyCollection<ContractorModel> matchedResult = matched;
            return Task.FromResult(matchedResult);
        }
    }
}
