namespace iTender.Integrator.Domain.Entities
{
    public class FinancialStatementModel
    {
        public Guid Id { get; set; }
        public Guid ContractorId { get; set; }
        public int Year { get; set; }
        public string? Month { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? CalculatedDate { get; set; }
        public Guid ApplicationId { get; set; }
        public DateTime? FyEnd { get; set; }
        public bool MeetsRegulations { get; set; }
        public decimal TurnoverInclVat { get; set; }
        public decimal Turnover { get; set; }
        public decimal NetAssetValue { get; set; }
    }
}
