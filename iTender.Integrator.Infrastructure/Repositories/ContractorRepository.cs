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

            if (model is not null)
            {
                PopulateAnnualTurnoverAndAvailableCapital(model);
                model.Grades = GetContractorGrades(model.Id).ToList();
            }

            return Task.FromResult(model);
        }

        public void PopulateAnnualTurnoverAndAvailableCapital(ContractorModel result, CancellationToken ct = default)
        {
            var query = new QueryExpression(CrmEntityNames.FinancialStatement)
            {
                ColumnSet = new ColumnSet(true),
                NoLock = true
            };

            query.Criteria.AddCondition(
                CrmFieldNames.FinancialStatementFields.ContractorId,
                ConditionOperator.Equal,
                result.Id);

            query.AddOrder(
                CrmFieldNames.FinancialStatementFields.Year,
                OrderType.Descending);
            var service = _crmServiceFactory.Create();
            var statement = service.RetrieveMultiple(query).Entities.Select(FinancialStatementMapper.ToDomain).ToList();

            if (statement.Count > 0 && statement[0].Id != Guid.Empty)
            {
                //get the annual turnover
                if (statement[0].TurnoverInclVat > 0 && statement[0].TurnoverInclVat != 0)
                {
                    result.AnnualTurnOver = statement[0].TurnoverInclVat;
                }
                else
                {
                    if (((statement[0].NetAssetValue > 0 && statement[0].NetAssetValue != 0) && (result.CurrentGrade == "3" || result.CurrentGrade == "4")))
                    {
                        result.AnnualTurnOver = CalculateMissingNotionalValue(statement[0].NetAssetValue);
                    }
                }

                //get the available capital
                if (statement[0].NetAssetValue > 0 && statement[0].NetAssetValue != 0)
                {
                    result.AvailableCapital = statement[0].NetAssetValue;
                }
                else
                {
                    if ((statement[0].TurnoverInclVat > 0 && statement[0].TurnoverInclVat != 0) && (result.CurrentGrade == "3" || result.CurrentGrade == "4"))
                    {
                        result.AvailableCapital = CalculateMissingNotionalValue(statement[0].TurnoverInclVat);
                    }
                }
            }
        }

        private decimal CalculateMissingNotionalValue(decimal availableValue)
        {
            decimal missingValue = 0;
            //
            decimal num1 = 0;
            //
            decimal num2 = 0;
            //
            num1 = Math.Round(((availableValue - 1000000) / (2000000 - 1000000)), 2);
            //
            num2 = ((200000 - 100000) / 1);
            //
            missingValue = num1 * num2 + 100000;

            return missingValue;

        }

        public List<ContractorGradeModel> GetContractorGrades(Guid contractorId)
        {
            var query = new QueryExpression(CrmEntityNames.ClassOfWork)
            {
                ColumnSet = new ColumnSet(
                    ContractorGradeFields.Id,
                    ContractorGradeFields.Name,
                    ContractorGradeFields.ContractorId,
                    ContractorGradeFields.ClassOfWorkTypeId,
                    ContractorGradeFields.ApprovedGrade,
                    ContractorGradeFields.ElectricalLicense,
                    ContractorGradeFields.CreatedOn,
                    ContractorGradeFields.ModifiedOn,
                    ContractorGradeFields.StateCode)
            };

            query.Criteria.AddCondition(
                ContractorGradeFields.ContractorId,
                ConditionOperator.Equal,
                contractorId);
            var service = _crmServiceFactory.Create();
            var entities = service.RetrieveMultiple(query).Entities;

            return entities
                .Select(ContractorGradeMapper.ToDomain)
                .Where(x => x != null)
                .Cast<ContractorGradeModel>()
                .ToList();
        }
    }
}
