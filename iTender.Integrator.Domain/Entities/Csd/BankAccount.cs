namespace iTender.Integrator.Domain.Entities.Csd
{
    public class BankAccount
    {
        public int BankAccountID { get; set; }

        public bool? IsPreferred { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsForeignBankAccount { get; set; }

        public string? AccountHolder { get; set; }

        public string? BankAccountTypeCode { get; set; }

        public string? BankName { get; set; }

        public string? BankCode { get; set; }

        public string? BranchName { get; set; }

        public string? BranchNumber { get; set; }

        public string? AccountNumber { get; set; }

        public string? BankAccountStatusCode { get; set; }

        public DateTime? BankAccountVerificationDate { get; set; }

        public string? AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; }

        public string? CountryCode { get; set; }

        public string? ZipCode { get; set; }

        public string? FirstName { get; set; }

        public string? Initials { get; set; }

        public string? LastName { get; set; }

        public string? Title { get; set; }

        public bool? IsIdentifierLinkedAtBank { get; set; }

        public bool? IsSharedFundingAccount { get; set; }

        public string? FundingContacts { get; set; }

        public string? Field1 { get; set; }

        public string? Field2 { get; set; }

        public string? Field3 { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? EditDate { get; set; }
    }
}
