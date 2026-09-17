namespace iTender.Integrator.Demo.Configuration
{
    public sealed class IntegratorApiOptions
    {
        public const string SectionName = "IntegratorApi";

        public string BaseUrl { get; set; } = string.Empty;

        public string ApiKey { get; set; } = string.Empty;
    }
}
