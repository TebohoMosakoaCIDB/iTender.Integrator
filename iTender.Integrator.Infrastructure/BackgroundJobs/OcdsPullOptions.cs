namespace iTender.Integrator.Infrastructure.BackgroundJobs
{
    public class OcdsPullOptions
    {
        public const string SectionName = "OcdsPull";
        public bool Enabled { get; set; } = true;
        public int IntervalMinutes { get; set; } = 15;
        public int InitialLookbackHours { get; set; } = 24;
        public int PageSize { get; set; } = 50;
        public int MaxPagesPerRun { get; set; } = 20;
        public int RetryBatchSize { get; set; } = 50;
    }
}
