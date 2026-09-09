namespace iTender.Integrator.Domain.Entities.Csd
{
    public class OwnershipNonNatural
    {
        public int OwnershipNonNaturalID { get; set; }

        public string? SupplierNumber { get; set; }

        public string? LegalName { get; set; }

        public string? TradingName { get; set; }

        public decimal SharesInterestPercentage { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? EditDate { get; set; }
    }
}
