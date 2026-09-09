namespace iTender.Integrator.Domain.Entities.Csd
{
    public class Bbee
    {
        public string? CertificateTypeCode { get; set; }

        public string? VerificationRegulatorCode { get; set; }

        public string? CertificateNumber { get; set; }

        public DateTime? CertificateIssueDate { get; set; }

        public DateTime? CertificateExpiryDate { get; set; }

        public string? StatusLevelOfContributorCode { get; set; }

        public decimal BlackOwnership { get; set; }

        public decimal BlackWomanOwnership { get; set; }

        public bool IsAcceptUnderstandAffidavid { get; set; }

        public string? CertificateSignedBy { get; set; }

        public DateTime? CertificateSignDate { get; set; }

        public string? SectorCharterCode { get; set; }

        public string? SubSectorCharterCode { get; set; }

        public bool ValueAddingSupplier { get; set; }

        public bool EmpoweringSupplier { get; set; }

        public string? IRBARegisteredAuditorCode { get; set; }

        public string? SANASAccreditedAgencyCode { get; set; }

        public decimal OwnershipScore { get; set; }

        public decimal ManagementControlScore { get; set; }

        public decimal EmploymentEquityScore { get; set; }

        public decimal SkillsDevelopmentScore { get; set; }

        public decimal PreferentialProcurementScore { get; set; }

        public decimal EnterpriseDevelopmentScore { get; set; }

        public decimal SocioEconomicDevelopmentScore { get; set; }

        public decimal EnterpriseSupplierDevelopmentScore { get; set; }

        public decimal LandOwnershipScore { get; set; }

        public decimal EmpowermentFinancingScore { get; set; }

        public decimal AccessFinancialServicesScore { get; set; }

        public decimal EconomicDevelopmentScore { get; set; }

        public decimal ForeignOwnershipScore { get; set; }

        public string? StatusTypeCode { get; set; }

        public DateTime? LastVerificationDate { get; set; }

        public string? Field1 { get; set; }

        public string? Field2 { get; set; }

        public string? Field3 { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? EditDate { get; set; }
    }
}
