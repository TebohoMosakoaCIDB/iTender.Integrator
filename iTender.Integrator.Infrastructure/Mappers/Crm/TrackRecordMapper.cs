using iTender.Integrator.Application.DTOs.Crm;
using Microsoft.Xrm.Sdk;
using static iTender.Integrator.Domain.Constants.CrmFieldNames;

namespace iTender.Integrator.Infrastructure.Mappers.Crm
{
    public static class TrackRecordMapper
    {
        public const string EntityName = "nv_trackrecord";

        public static class Fields
        {
            public const string Id = "nv_trackrecordid";
            public const string ContractorId = "nv_contractorid";

            public const string ContractId = "nv_constructioncontractid";
            public const string ContractorCrsNumber = "nv_contractorcrsnumber";
            public const string Name = "nv_name";

            public const string ContractorShareValue = "nv_contractorsshareinclvat";
        }
        public static TrackRecordModel ToDomain(Entity e)
        {
            if (e == null) return null!;

            var employerRef =
                e.GetAttributeValue<EntityReference>("nv_employer_contract_id");

            Console.WriteLine($"Employer Ref ID: {employerRef?.Id}");
            Console.WriteLine($"Employer Ref Name: {employerRef?.Name}");

            return new TrackRecordModel
            {
                Id = e.Id,

                Reference = e.GetAttributeValue<string>(Fields.ContractorCrsNumber),
                Title = e.GetAttributeValue<string>(Fields.Name),
                ContractId = e.GetAttributeValue<Guid>(Fields.ContractorId),

                Value = e.GetAttributeValue<Money>(Fields.ContractorShareValue)?.Value,

                EmployerText =
                    (e.GetAttributeValue<AliasedValue>("contract.nv_employer_contract_id")
                        ?.Value as EntityReference)
                    ?.Name
            };
        }
        public static Entity ToEntity(TrackRecordModel domain)
        {
            var entity = new Entity(EntityName);

            if (domain.ContractorId.HasValue)
                entity["nv_contractorid"] = new EntityReference("nv_contractor", domain.ContractorId.Value);

            if (domain.ClassOfWorkId.HasValue)
                entity["nv_classofworkid"] = new EntityReference("nv_classofwork", domain.ClassOfWorkId.Value);

            if (domain.ContractId.HasValue)
                entity["nv_constructioncontractid"] =
                    new EntityReference("nv_contract", domain.ContractId.Value);

            if (domain.ContractorShareInclVat.HasValue)
                entity["nv_contractorsshareinclvat"] =
                    new Money(domain.ContractorShareInclVat.Value);

            return entity;
        }
    }
}
