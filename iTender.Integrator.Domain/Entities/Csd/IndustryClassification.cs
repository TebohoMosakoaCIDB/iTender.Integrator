namespace iTender.Integrator.Domain.Entities.Csd
{
    public class IndustryClassification
    {
        public string? IndustryClassificationCode { get; set; }

        public decimal PercentageRanking { get; set; }

        public bool CoreIndustryIndicator { get; set; }
    }
}
