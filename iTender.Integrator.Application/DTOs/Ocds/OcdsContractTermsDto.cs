namespace iTender.Integrator.Application.DTOs.Ocds
{
    public class OcdsContractTermsDto
    {
        public bool ReservedExecution { get; set; }

        public string? PerformanceTerms { get; set; }

        public bool HasElectronicOrdering { get; set; }

        public string? ElectronicInvoicingPolicy { get; set; }

        public bool HasElectronicPayment { get; set; }
    }
}
