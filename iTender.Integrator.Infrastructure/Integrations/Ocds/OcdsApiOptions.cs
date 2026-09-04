namespace iTender.Integrator.Infrastructure.Integrations.Ocds
{
    public class OcdsApiOptions
    {
        public const string SectionName = "OcdsApi";

        public string BaseUrl { get; set; } = string.Empty;

        public string ReleasesEndpoint { get; set; } = string.Empty;

        public int TimeoutSeconds { get; set; } = 60;
    }
}
