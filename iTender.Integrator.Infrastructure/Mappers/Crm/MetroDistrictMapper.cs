using iTender.Integrator.Application.DTOs.Crm;
using Microsoft.Xrm.Sdk;
using static iTender.Integrator.Domain.Constants.CrmFieldNames;

namespace iTender.Integrator.Infrastructure.Mappers.Crm
{
    public static class MetroDistrictMapper
    {
        public static MetroDistrictModel? ToDomain(Entity entity)
        {
            if (entity is null) return null;

            return new MetroDistrictModel
            {
                Id = entity.Id,
                ProvinceId = entity.GetAttributeValue<EntityReference>(MetroDistrictFields.ProvinceId)?.Id,
                StateCode = entity.GetAttributeValue<OptionSetValue>(MetroDistrictFields.StateCode)?.Value,
                Name = entity.GetAttributeValue<string>(MetroDistrictFields.Name)
            };
        }

        public static Entity ToEntity(MetroDistrictModel model)
        {
            var entity = model.Id != Guid.Empty
                ? new Entity(Domain.Constants.CrmEntityNames.MetroDistrict, model.Id)
                : new Entity(Domain.Constants.CrmEntityNames.MetroDistrict);

            entity[MetroDistrictFields.Name] = model.Name;

            if (model.ProvinceId.HasValue)
                entity[MetroDistrictFields.ProvinceId] =
                    new EntityReference(Domain.Constants.CrmEntityNames.Province, model.ProvinceId.Value);

            if (model.StateCode.HasValue)
                entity[MetroDistrictFields.StateCode] = new OptionSetValue(model.StateCode.Value);

            return entity;
        }
    }
}