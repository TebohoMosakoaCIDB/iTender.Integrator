using System.Xml.Serialization;

namespace iTender.Integrator.Domain.Entities.Csd
{
    public class SupplierIdentification
    {
        public string? SupplierNumber { get; set; }
        public Guid? UniqueRegistrationReferenceNumber { get; set; }
        public bool IsActive { get; set; }
        public string? SupplierInactiveReason { get; set; }
        public DateTime? SupplierInactiveDate { get; set; }
        public string? SupplierStateCode { get; set; }
        public bool IsAssociated { get; set; }
        public string? SupplierTypeCode { get; set; }
        public string? SupplierSubTypeCode { get; set; }
        public string? GovernmentTypeCode { get; set; }
        public string? CountryOfOriginCode { get; set; }
        public string? LegalName { get; set; }
        public string? TradingName { get; set; }
        public string? IDTypeCode { get; set; }
        public string? SAIDNumber { get; set; }
        public string? ForeignIDNumber { get; set; }
        public string? ForeignPassportNumber { get; set; }
        public string? WorkPermitNumber { get; set; }
        public string? SACompanyNumber { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string? ForeignCompanyRegistrationNumber { get; set; }
        public string? SATrustRegistrationNumber { get; set; }
        public string? ForeignTrustRegistrationNumber { get; set; }
        public string? NonProfitOrganisationNumber { get; set; }
        public string? OoSIDNumber { get; set; }
        public DateTime? DateOperationsStarted { get; set; }
        public bool HaveBankAccount { get; set; }
        public string? BusinessStatusCode { get; set; }
        public DateTime? BusinessStatusLastVerificationDate { get; set; }
        public bool IsListedOnStockExchange { get; set; }
        public bool IsOwnedByNaturalSAPerson { get; set; }
        public string? TotalAnnualTurnoverCode { get; set; }
        public DateTime? FinancialYearStartDate { get; set; }
        public string? Field1 { get; set; }
        public string? Field2 { get; set; }
        public string? Field3 { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? EditDate { get; set; }
        [XmlArray("SupplierFlags")]
        [XmlArrayItem("SupplierFlag")]
        public List<SupplierFlag> SupplierFlags { get; set; } = [];
    }
}
