namespace iTender.Integrator.Domain.Entities.Csd
{
    public class Tax
    {
        public bool IsRegistered { get; set; }

        public bool IsVATVendor { get; set; }

        public string? IncomeTaxNumber { get; set; }

        public string? PAYENumber { get; set; }

        public string? VATNumber { get; set; }

        public bool IsValidCertificate { get; set; }

        public string? ValidationResponse { get; set; }

        public DateTime? LastVerificationDate { get; set; }

        public string? Field1 { get; set; }

        public string? Field2 { get; set; }

        public string? Field3 { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? EditDate { get; set; }
    }
}
