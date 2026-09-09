namespace iTender.Integrator.Domain.Entities.Csd
{
    public class Contact
    {
        public int ContactID { get; set; }

        public List<ContactType> ContactTypes { get; set; } = [];

        public bool IsPreferred { get; set; }

        public string? Name { get; set; }

        public string? Surname { get; set; }

        public string? IdentificationTypeCode { get; set; }

        public string? SAIDNumber { get; set; }

        public string? ForeignIDNumber { get; set; }

        public string? ForeignPassportNumber { get; set; }

        public string? WorkPermitNumber { get; set; }

        public bool PreferCellphone { get; set; }

        public bool PreferEmail { get; set; }

        public bool PreferFax { get; set; }

        public bool PreferPostal { get; set; }

        public bool PreferSMS { get; set; }

        public bool PreferTelephone { get; set; }

        public string? EmailAddress { get; set; }

        public string? CellphoneNumber { get; set; }

        public string? FaxNumber { get; set; }

        public string? TelephoneNumber { get; set; }

        public string? TollFreeNumber { get; set; }

        public string? WebsiteAddress { get; set; }

        public string? FundingPartnerLegalName { get; set; }

        public bool CSDUser { get; set; }

        public bool IsActive { get; set; }

        public string? Field1 { get; set; }

        public string? Field2 { get; set; }

        public string? Field3 { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? EditDate { get; set; }
    }
}
