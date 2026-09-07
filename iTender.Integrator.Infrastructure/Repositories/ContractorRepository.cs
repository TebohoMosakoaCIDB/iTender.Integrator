using iTender.Integrator.Application.DTOs.Crm;
using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Domain.Constants;
using iTender.Integrator.Domain.Entities;
using iTender.Integrator.Infrastructure.Integrations.CRM;
using iTender.Integrator.Infrastructure.Mappers.Crm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace iTender.Integrator.Infrastructure.Repositories
{
    public class ContractorRepository : IContractorRepository
    {
        private readonly IOrganizationService _service;

        public ContractorRepository(ICrmServiceFactory crmFactory)
        {
            _service = crmFactory.Create();
        }
        public async Task<ContractorModel?> GetByCidbRegistrationNumberAsync(string registrationNumber, CancellationToken cancellationToken = default)
        {

            var query = new QueryExpression(CrmEntityNames.Account)
            {
                ColumnSet = new ColumnSet(true),
                Distinct = true,
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(CrmFieldNames.ContractorFields.CrsNumber, ConditionOperator.Equal, registrationNumber),
                    }
                }
            };

            var entity = await Task.Run(() =>
                _service.RetrieveMultiple(query).Entities.FirstOrDefault(), cancellationToken);

            return entity == null
                ? null
                : ContractorMapper.ToDomain(entity);
        }

        public async Task<ContractorModel?> GetByDynamicsAccountIdAsync(Guid dynamicsAccountId, CancellationToken cancellationToken = default)
        {
            var entity = await Task.Run(() =>
                _service.Retrieve(
                    CrmEntityNames.Account,
                    dynamicsAccountId,
                    new ColumnSet(true)), cancellationToken);

            return entity == null
                ? null
                : ContractorMapper.ToDomain(entity);
        }

        public async Task<IReadOnlyCollection<ContractorModel>> GetEligibleForAsync(string classOfWork, int minGrade, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
