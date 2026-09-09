using iTender.Integrator.Application.DTOs.Crm;
using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Domain.Constants;
using iTender.Integrator.Infrastructure.Integrations.CRM;
using iTender.Integrator.Infrastructure.Mappers.Crm;
using Microsoft.Xrm.Sdk.Query;
using static iTender.Integrator.Domain.Constants.CrmFieldNames;

namespace iTender.Integrator.Infrastructure.Repositories
{
    public class ContractorGradeRepository : IContractorGradeRepository
    {
        private readonly ICrmServiceFactory _crmServiceFactory;

        public ContractorGradeRepository(ICrmServiceFactory crmServiceFactory)
        {
            _crmServiceFactory = crmServiceFactory;
        }

        public Task<ContractorModel?> GetByCsdNumberAsync(
            string csdNumber,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(csdNumber))
                throw new ArgumentException("CSD number is required.", nameof(csdNumber));

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
                    ContractorFields.PotentiallyEmerging,
                    ContractorFields.Phone,
                    ContractorFields.Email,
                    ContractorFields.BBBEEEStatus,
                    ContractorFields.IsSanctioned,
                    ContractorFields.PreviouslySanctioned,
                    ContractorFields.Moratorium,
                    ContractorFields.PrimaryContactId,
                    ContractorFields.StatusCode,
                    ContractorFields.EnterpriseType,
                    ContractorFields.ActivationDate,
                    ContractorFields.RenewalDueDate,
                    ContractorFields.EnterpriseRegistrationNumber),
                TopCount = 1
            };

            query.Criteria.AddCondition(
                ContractorFields.CSDNumber,
                ConditionOperator.Equal,
                csdNumber.Trim());

            // IOrganizationService.RetrieveMultiple is a synchronous SDK call - there's
            // no async surface on this interface. Running it here rather than pushing
            // Task.Run onto the caller keeps the blocking-call concern in one place.
            var service = _crmServiceFactory.Create();
            var result = service.RetrieveMultiple(query);

            var entity = result.Entities.FirstOrDefault();
            var model = entity is null ? null : ContractorMapper.ToDomain(entity);

            return Task.FromResult(model);
        }
    }
}
