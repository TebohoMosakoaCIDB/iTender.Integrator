using iTender.Integrator.Application.DTOs.Crm;
using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Domain.Constants;
using iTender.Integrator.Infrastructure.Integrations.CRM;
using iTender.Integrator.Infrastructure.Mappers.Crm;
using Microsoft.Xrm.Sdk.Query;
using static iTender.Integrator.Domain.Constants.CrmFieldNames;

namespace iTender.Integrator.Infrastructure.Repositories
{
    public class ContractorRepository : IContractorRepository
    {
        private readonly ICrmServiceFactory _crmServiceFactory;

        public ContractorRepository(ICrmServiceFactory crmServiceFactory)
        {
            _crmServiceFactory = crmServiceFactory;
        }

        public Task<ContractorModel?> GetByCrsNumberAsync(
             string crsNumber,
             CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(crsNumber))
                throw new ArgumentException("CRS number is required.", nameof(crsNumber));

            return FindByAsync(ContractorFields.CrsNumber, crsNumber);
        }

        private Task<ContractorModel?> FindByAsync(string filterField, string filterValue)
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

            query.Criteria.AddCondition(filterField, ConditionOperator.Equal, filterValue.Trim());

            var service = _crmServiceFactory.Create();
            var result = service.RetrieveMultiple(query);

            var entity = result.Entities.FirstOrDefault();
            var model = entity is null ? null : ContractorMapper.ToDomain(entity);

            return Task.FromResult(model);
        }
    }
}
