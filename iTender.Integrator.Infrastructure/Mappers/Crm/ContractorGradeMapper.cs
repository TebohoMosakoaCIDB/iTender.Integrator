using iTender.Integrator.Application.DTOs.Crm;
using Microsoft.Xrm.Sdk;
using static iTender.Integrator.Domain.Constants.CrmFieldNames;

namespace iTender.Integrator.Infrastructure.Mappers.Crm
{
    public static class ContractorGradeMapper
    {
        public static ContractorGradeModel? ToDomain(Entity entity)
        {
            if (entity == null) return null;

            return new ContractorGradeModel
            {
                ClassOfWorkId =
                    entity.GetAttributeValue<EntityReference>(ContractorGradeFields.ClassOfWorkTypeId)?.Id
                    ?? entity.GetAttributeValue<Guid>(ContractorGradeFields.Id),

                ClassOfWorksDescription =
                    entity.GetAttributeValue<EntityReference>(ContractorGradeFields.ClassOfWorkTypeId)?.Name
                    ?? entity.GetAttributeValue<string>(ContractorGradeFields.Name),

                ElectricalLicense = entity.FormattedValues.Contains(ContractorGradeFields.ElectricalLicense)
                    ? entity.FormattedValues[ContractorGradeFields.ElectricalLicense]
                    : entity.GetAttributeValue<OptionSetValue>(ContractorGradeFields.ElectricalLicense)?.Value.ToString(),

                ApprovedGrade = entity.FormattedValues.Contains(ContractorGradeFields.ApprovedGrade)
                    ? entity.FormattedValues[ContractorGradeFields.ApprovedGrade]
                    : entity.GetAttributeValue<OptionSetValue>(ContractorGradeFields.ApprovedGrade)?.Value.ToString(),

                StatusText = entity.FormattedValues.Contains("statecode")
                    ? entity.FormattedValues["statecode"]
                    : null,


                DateOfRegistration =
                    entity.GetAttributeValue<DateTime?>(ContractorGradeFields.CreatedOn),

                DateUpdated =
                    entity.GetAttributeValue<DateTime?>(ContractorGradeFields.ModifiedOn)
            };
        }
    }
}
