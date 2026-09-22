using iTender.Integrator.Application.DTOs.Crm;
using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Infrastructure.Integrations.CRM;
using iTender.Integrator.Infrastructure.Mappers.Crm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace iTender.Integrator.Infrastructure.Repositories
{
    public class TrackRecordRepository : ITrackRecordRepository
    {
        private readonly ICrmServiceFactory _crmServiceFactory;

        public TrackRecordRepository(ICrmServiceFactory crmServiceFactory)
        {
            _crmServiceFactory = crmServiceFactory;
        }

        public async Task<IReadOnlyCollection<TrackRecordModel>> GetByCrsNumberAsync(string crsNumber, CancellationToken cancellationToken = default)
        {
            var query = new QueryExpression(TrackRecordMapper.EntityName) // "nv_trackrecord"
            {
                ColumnSet = new ColumnSet(
                    TrackRecordMapper.Fields.Name,                  // nv_name → Title
                    TrackRecordMapper.Fields.ContractorShareValue,  // nv_contractorsshareinclvat → Value
                    TrackRecordMapper.Fields.ContractId,            // nv_constructioncontractid → FK
                    "statecode"
                ),
                NoLock = true,
                Distinct = false
            };

            // Filter by contractor CRS number
            query.Criteria.AddCondition(
                TrackRecordMapper.Fields.ContractorCrsNumber,
                ConditionOperator.Equal,
                crsNumber);

            // Exclude inactive records
            query.Criteria.AddCondition(
                "statecode",
                ConditionOperator.NotEqual,
                1);

            // ── Link to nv_contract for title, number, completion date, total value ──
            var contractLink = new LinkEntity
            {
                LinkFromEntityName = TrackRecordMapper.EntityName,
                LinkFromAttributeName = TrackRecordMapper.Fields.ContractId,
                LinkToEntityName = "nv_contract",
                LinkToAttributeName = "nv_contractid",
                JoinOperator = JoinOperator.LeftOuter, // LEFT so we don't lose records
                Columns = new ColumnSet(
                    "nv_title",
                    "nv_contractnumber",
                    "nv_totalcontractvalueinclvat",
                    "nv_dateofpracticalcompletion"
                ),
                EntityAlias = "contract"
            };

            // ── Link from contract to employer (account) for the client name ──
            var employerLink = new LinkEntity
            {
                LinkFromEntityName = "nv_contract",
                LinkFromAttributeName = "nv_employer_contract_id",
                LinkToEntityName = "account",
                LinkToAttributeName = "accountid",
                JoinOperator = JoinOperator.LeftOuter,
                Columns = new ColumnSet("name"),
                EntityAlias = "employer"
            };

            contractLink.LinkEntities.Add(employerLink);
            query.LinkEntities.Add(contractLink);

            var service = _crmServiceFactory.Create();
            var results = service.RetrieveMultiple(query);

            return results.Entities.Select(e =>
            {
                // Value: prefer the contract's total (incl. VAT); fall back to
                // the track record's contractor share.
                var totalValue = e.Contains("contract.nv_totalcontractvalueinclvat")
                    ? ((AliasedValue)e["contract.nv_totalcontractvalueinclvat"]).Value as Money
                    : null;

                var contractorShare = e.GetAttributeValue<Money>(
                    TrackRecordMapper.Fields.ContractorShareValue);

                var contractTitle = e.Contains("contract.nv_title")
                    ? ((AliasedValue)e["contract.nv_title"]).Value?.ToString()
                    : null;

                var contractNumber = e.Contains("contract.nv_contractnumber")
                    ? ((AliasedValue)e["contract.nv_contractnumber"]).Value?.ToString()
                    : null;

                var completedOn = e.Contains("contract.nv_dateofpracticalcompletion")
                    ? ((AliasedValue)e["contract.nv_dateofpracticalcompletion"]).Value as DateTime?
                    : null;

                var employerName = e.Contains("employer.name")
                    ? ((AliasedValue)e["employer.name"]).Value?.ToString()
                    : null;

                return new TrackRecordModel
                {
                    Id = e.Id,

                    ContractId = e.GetAttributeValue<EntityReference>(
                        TrackRecordMapper.Fields.ContractId)?.Id,

                    Title = e.GetAttributeValue<string>(TrackRecordMapper.Fields.Name)
                            ?? contractTitle,           // fall back to contract title if record title empty

                    Reference = contractNumber,

                    EmployerText = employerName,

                    Value = totalValue?.Value ?? contractorShare?.Value,

                    ContractorShareInclVat = contractorShare?.Value,

                    CompletedOn = completedOn,

                    StateCode = e.GetAttributeValue<OptionSetValue>("statecode")?.Value
                };
            }).ToList();
        }
    }
}
