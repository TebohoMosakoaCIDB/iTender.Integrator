namespace iTender.Integrator.Application.DTOs.Crm
{
    public class TrackRecordModel
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

        // NEW — needed for the mockup's "Completed" column
        public DateTime? CompletedOn { get; set; }

        // Optional — allows future performance tracking
        public decimal? PerformanceScore { get; set; }
    }

    public sealed class TrackRecordSummary
    {
        public int ProjectsCompleted { get; set; }
        public int ContractsAwarded { get; set; }
        public decimal AveragePerformance { get; set; }
        public int ClaimsRaised { get; set; }
        public int Disputes { get; set; }
        public int SafetyIncidents { get; set; }
    }
}
