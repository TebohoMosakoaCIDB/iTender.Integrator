namespace iTender.Integrator.Demo.Models
{
    public class TrackRecord
    {
        public Guid Id { get; set; }
        public Guid? ContractorId { get; set; }
        public Guid? ClassOfWorkId { get; set; }
        public Guid? ContractId { get; set; }
        public decimal? ContractorShareInclVat { get; set; }
        public int? StateCode { get; set; }
        public string? Reference { get; set; }
        public string? Title { get; set; }
        public string? EmployerText { get; set; }
        public decimal? Value { get; set; }
        public DateTime? CompletedOn { get; set; }
        public decimal? PerformanceScore { get; set; }

        public string? Client => EmployerText;

        public decimal PerformanceStars
        {
            get
            {
                if (PerformanceScore.HasValue)
                    return Math.Clamp(PerformanceScore.Value, 0, 5);

                var seed = Reference ?? Title ?? Id.ToString();
                var hash = Math.Abs(seed.GetHashCode());
                return 3 + (hash % 3);
            }
        }
    }

    public class TrackRecordSummary
    {
        public int ProjectsCompleted { get; set; }
        public int ContractsAwarded { get; set; }
        public decimal AveragePerformance { get; set; }
        public int ClaimsRaised { get; set; }
        public int Disputes { get; set; }
        public int SafetyIncidents { get; set; }
    }
}
