using iTender.Integrator.Application.DTOs.Crm;
using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Domain.Constants;
using iTender.Integrator.Infrastructure.Integrations.CRM;
using iTender.Integrator.Infrastructure.Mappers.Crm;
using Microsoft.Xrm.Sdk.Query;
using static iTender.Integrator.Domain.Constants.CrmFieldNames;

namespace iTender.Integrator.Infrastructure.Repositories
{
    public class MetroDistrictRepository : IMetroDistrictRepository
    {
        private readonly ICrmServiceFactory _crmServiceFactory;

        public MetroDistrictRepository(ICrmServiceFactory crmServiceFactory)
        {
            _crmServiceFactory = crmServiceFactory;
        }

        public Task<IReadOnlyList<MetroDistrictModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var query = new QueryExpression(CrmEntityNames.MetroDistrict)
            {
                ColumnSet = new ColumnSet(
                    MetroDistrictFields.Id, MetroDistrictFields.Name,
                    MetroDistrictFields.ProvinceId, MetroDistrictFields.StateCode)
            };

            return Task.FromResult(RunQuery(query));
        }

        public Task<IReadOnlyList<MetroDistrictModel>> GetByProvinceAsync(
            Guid provinceId, CancellationToken cancellationToken = default)
        {
            var query = new QueryExpression(CrmEntityNames.MetroDistrict)
            {
                ColumnSet = new ColumnSet(
                    MetroDistrictFields.Id, MetroDistrictFields.Name,
                    MetroDistrictFields.ProvinceId, MetroDistrictFields.StateCode)
            };

            query.Criteria.AddCondition(MetroDistrictFields.ProvinceId, ConditionOperator.Equal, provinceId);
            // statecode 0 = Active, per the pattern this was built from.
            query.Criteria.AddCondition(MetroDistrictFields.StateCode, ConditionOperator.Equal, 0);

            return Task.FromResult(RunQuery(query));
        }

        public async Task<MetroDistrictModel?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var all = await GetAllAsync(cancellationToken);

            return all.FirstOrDefault(m =>
                string.Equals(m.Name?.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        private IReadOnlyList<MetroDistrictModel> RunQuery(QueryExpression query)
        {
            var service = _crmServiceFactory.Create();
            var result = service.RetrieveMultiple(query);

            return result.Entities
                .Select(MetroDistrictMapper.ToDomain)
                .Where(m => m is not null)
                .Select(m => m!)
                .ToList();
        }
    }
}