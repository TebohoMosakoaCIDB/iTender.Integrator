using iTender.Integrator.Application.DTOs.Crm;
using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Domain.Constants;
using iTender.Integrator.Infrastructure.Integrations.CRM;
using iTender.Integrator.Infrastructure.Mappers.Crm;
using Microsoft.Xrm.Sdk.Query;
using static iTender.Integrator.Domain.Constants.CrmFieldNames;

namespace iTender.Integrator.Infrastructure.Repositories
{
    public class ProvinceRepository : IProvinceRepository
    {
        private readonly ICrmServiceFactory _crmServiceFactory;

        public ProvinceRepository(ICrmServiceFactory crmServiceFactory)
        {
            _crmServiceFactory = crmServiceFactory;
        }

        public Task<IReadOnlyList<ProvinceModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var query = new QueryExpression(CrmEntityNames.Province)
            {
                ColumnSet = new ColumnSet(ProvinceFields.Id, ProvinceFields.Name, ProvinceFields.SourceId)
            };

            query.Criteria.AddCondition(ProvinceFields.Name, ConditionOperator.NotNull);
            query.AddOrder(ProvinceFields.Name, OrderType.Ascending);

            var service = _crmServiceFactory.Create();
            var result = service.RetrieveMultiple(query);

            IReadOnlyList<ProvinceModel> models = result.Entities
                .Select(ProvinceMapper.ToDomain)
                .Where(m => m is not null)
                .Select(m => m!)
                .ToList();

            return Task.FromResult(models);
        }

        public async Task<ProvinceModel?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            // 11 records total (9 provinces + "Foreign" + one unnamed) - fetching all
            // and matching in memory is simpler and cheaper than a live query per
            // lookup, and avoids CRM collation quirks with exact-match ConditionOperator.
            var all = await GetAllAsync(cancellationToken);

            return all.FirstOrDefault(p =>
                string.Equals(p.Name?.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}