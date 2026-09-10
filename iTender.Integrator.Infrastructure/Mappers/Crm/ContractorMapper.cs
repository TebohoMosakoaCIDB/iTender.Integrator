using iTender.Integrator.Application.DTOs.Crm;
using iTender.Integrator.Domain.Constants;
using iTender.Integrator.Domain.Enums;
using Microsoft.Xrm.Sdk;
using static iTender.Integrator.Domain.Constants.CrmFieldNames;

namespace iTender.Integrator.Infrastructure.Mappers.Crm
{
    public class ContractorMapper
    {
        public static Entity ToEntity(ContractorModel domain)
        {
            var entity = new Entity(CrmEntityNames.Account);

            entity[CrmFieldNames.ContractorFields.Name] = domain.Name;
            entity[CrmFieldNames.ContractorFields.CrsNumber] = domain.CrsNumber;


            entity[CrmFieldNames.ContractorFields.PotentiallyEmerging] = domain.IsPotentiallyEmerging;

            entity[CrmFieldNames.ContractorFields.EnterpriseRegistrationNumber] =
                domain.EnterpriseRegistrationNumber;

            entity[CrmFieldNames.ContractorFields.BBBEEEStatus] =
                domain.BBBEEstatusText;

            return entity;
        }

        public static ContractorModel? ToDomain(Entity entity)
        {
            if (entity == null) return null;

            var model = new ContractorModel
            {
                Id = entity.Id,

                Name = GetString(entity, ContractorFields.Name),

                EnterpriseName = GetString(entity, ContractorFields.Name),

                TradingAs = GetString(entity, ContractorFields.TradingAs),

                CrsNumber = GetString(entity, ContractorFields.CrsNumber),

                CsdNumber = GetString(entity, ContractorFields.CSDNumber),

                ProvinceId = entity.GetAttributeValue<EntityReference?>(ContractorFields.ProvinceId)?.Id.ToString(),

                ProvinceText = entity.GetAttributeValue<EntityReference?>(ContractorFields.ProvinceId)?.Name,

                //CurrentContractorGrade = GetOptionSetText(entity, ContractorFields.CurrentContractorGrade),

                CurrentContractorGradingDesignation = GetString(entity, ContractorFields.CurrentContractorGradingDesignation),

                IsPotentiallyEmerging = GetBool(entity, ContractorFields.PotentiallyEmerging),

                Phone = GetString(entity, ContractorFields.Phone),

                Email = GetString(entity, ContractorFields.Email),

                BBBEEstatusText = ((BBBEEStatus)Convert.ToInt32(GetOptionSetText(entity, ContractorFields.BBBEEEStatus))).ToString(),

                IsSanctioned = GetBool(entity, ContractorFields.IsSanctioned),

                PreviouslySanctioned = GetBool(entity, ContractorFields.PreviouslySanctioned),

                IsMoratorium = GetBool(entity, ContractorFields.Moratorium),

                PrimaryContactId = entity.GetAttributeValue<EntityReference?>(ContractorFields.PrimaryContactId)?.Id,

                BankingDetails = new BankModel
                {
                    BankName = GetString(entity, ContractorFields.BankName),
                    BankAccountNumber = GetString(entity, ContractorFields.BankAccountNumber),
                    BankAccountName = GetString(entity, ContractorFields.BankAccountName),
                    BranchCode = GetString(entity, ContractorFields.BranchCode),
                },

                Address1 = new AddressModel
                {
                    Line1 = GetString(entity, ContractorFields.AddressLine1),
                    City = GetString(entity, ContractorFields.AddressCity),
                    Province = GetString(entity, ContractorFields.AddressProvince),
                    PostalCode = GetString(entity, ContractorFields.AddressPostalCode)
                },

                Address2 = new AddressModel
                {
                    Line1 = GetString(entity, ContractorFields.PostalAddressLine1),
                    City = GetString(entity, ContractorFields.PostalAddressCity),
                    Province = GetString(entity, ContractorFields.PostalAddressProvince),
                    PostalCode = GetString(entity, ContractorFields.PostalAddressPostalCode)
                },

                ActivationDate = GetDate(entity, ContractorFields.ActivationDate),

                RenewalDueDate = GetDate(entity, ContractorFields.RenewalDueDate),

                CreatedOn = GetDate(entity, ContractorFields.CreatedOn),

                ModifiedOn = GetDate(entity, ContractorFields.ModifiedOn),

                ExpiryData = GetDate(entity, ContractorFields.HdiExpiryDate),

                EnterpriseRegistrationNumber = GetString(entity, ContractorFields.EnterpriseRegistrationNumber)
            };

            if (entity.FormattedValues.Contains(ContractorFields.StatusCode))
            {
                model.StatusText = entity.FormattedValues[ContractorFields.StatusCode];
            }

            if (entity.FormattedValues.Contains(
                CrmFieldNames.ContractorFields.EnterpriseType))
            {
                model.EnterpriseTypeText =
                    entity.FormattedValues[
                        CrmFieldNames.ContractorFields.EnterpriseType];
            }

            return model;
        }

        private static string GetString(Entity e, string key)
        => e.Contains(key) ? e[key]?.ToString() : null;

        private static bool GetBool(Entity e, string key)
            => e.Contains(key) && e[key] is bool b && b;

        private static DateTime? GetDate(Entity e, string key)
            => e.Contains(key) && DateTime.TryParse(e[key]?.ToString(), out var dt)
                ? dt
                : null;

        private static string GetOptionSetText(Entity e, string key)
        {
            if (!e.Contains(key)) return null;

            return e[key] switch
            {
                OptionSetValue osv => osv.Value.ToString(),
                _ => e[key]?.ToString()
            };
        }

        private static decimal? GetMoney(Entity e, string key)
        {
            if (!e.Contains(key)) return null;

            return e[key] switch
            {
                Money m => m.Value,
                _ => null
            };
        }

        private static Guid? GetEntityReferenceId(Entity e, string key)
        {
            if (!e.Contains(key)) return null;

            return e[key] switch
            {
                EntityReference er => er.Id,
                _ => null
            };
        }
    }
}
