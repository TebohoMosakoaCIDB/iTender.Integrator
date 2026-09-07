using System;
using System.Collections.Generic;
using System.Text;

namespace iTender.Integrator.Application.DTOs.Crm
{
    public class ContractorModel
    {
        public Guid Id { get; set; }
        public string? TradingAs { get; set; }
        public string? Name { get; set; }
        public string? EnterpriseName { get; set; }
        public string? EnterpriseRegistrationNumber { get; set; }
        public string? CsdNumber { get; set; }
        public string? CrsNumber { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool PreviouslySanctioned { get; set; }
        public bool IsPotentiallyEmerging { get; set; }
        public bool IsMoratorium { get; set; }
        public bool EElicense { get; set; }
        public string? StatusText { get; set; }
        public string? ProvinceId { get; set; }
        public string? ProvinceText { get; set; }
        public BankModel? BankingDetails { get; set; }
        public AddressModel? Address1 { get; set; }
        public AddressModel? Address2 { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? ActivationDate { get; set; }

        public DateTime? RenewalDueDate { get; set; }
        public DateTime? ExpiryData { get; set; } // sic - matches a typo in the source model
        public string? CurrentContractorGradingDesignation { get; set; }
        public string? CurrentGrade { get; set; }
        public string? CurrentGradingDesignation { get; set; }
        public decimal AnnualTurnOver { get; set; }
        public decimal NetAssetValue { get; set; }
        public decimal LargestContractValue { get; set; }
        public decimal AvailableCapital { get; set; }
        public int Professionals { get; set; }
        public IEnumerable<ContractorGradeModel>? Grades { get; set; }
        public string? BBBEEstatusText { get; set; }
        public Guid? PrimaryContactId { get; set; }
        public string? ContactPersonName { get; set; }
        public string? ContactPersonEmailAddress { get; set; }
        public string? ContactPersonTelephone { get; set; }
        public string? ContactPersonMobileNumber { get; set; }
        public string? EnterpriseTypeText { get; set; }
    }
}
