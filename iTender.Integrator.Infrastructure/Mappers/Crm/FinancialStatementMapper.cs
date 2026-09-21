using iTender.Integrator.Domain.Constants;
using iTender.Integrator.Domain.Entities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace iTender.Integrator.Infrastructure.Mappers.Crm
{
    public static class FinancialStatementMapper
    {
        public static FinancialStatementModel ToDomain(Entity e)
        {
            if (e == null) return null!;

            return new FinancialStatementModel
            {
                Id = e.Id,

                ApplicationId =
                    e.GetAttributeValue<EntityReference>(CrmFieldNames.FinancialStatementFields.ApplicationId)?.Id ?? Guid.Empty,

                ContractorId =
                    e.GetAttributeValue<EntityReference>(CrmFieldNames.FinancialStatementFields.ContractorId)?.Id ?? Guid.Empty,

                Year =
                    e.GetAttributeValue<int?>(CrmFieldNames.FinancialStatementFields.Year) ?? 0,

                CreatedOn =
                    e.GetAttributeValue<DateTime?>(CrmFieldNames.FinancialStatementFields.CreatedOn),

                TurnoverInclVat =
                    e.GetAttributeValue<Money>(CrmFieldNames.FinancialStatementFields.TurnoverInclVat)?.Value ?? 0m,

                Turnover =
                    e.GetAttributeValue<Money>(CrmFieldNames.FinancialStatementFields.Turnover)?.Value ?? 0m,

                NetAssetValue =
                    e.GetAttributeValue<Money>(CrmFieldNames.FinancialStatementFields.NetAssetValue)?.Value ?? 0m,

                MeetsRegulations =
                    e.GetAttributeValue<bool?>(CrmFieldNames.FinancialStatementFields.MeetsRegulations) ?? false
            };
        }
    }
}
