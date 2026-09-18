namespace iTender.Integrator.Infrastructure.Integrations.eTenders
{
    public class ETendersAdminApiOptions
    {
        public const string SectionName = "ETendersAdmin";

        public string BaseUrl { get; set; } = "https://admin-uat.etenders.gov.za/";

        public string CreateTenderEndpoint { get; set; } = "api/Etenders/add";

        public int TimeoutSeconds { get; set; } = 60;

        // UNCONFIRMED: the OpenAPI spec provided has no security scheme at all,
        // which is very unlikely to be the real picture for an endpoint that
        // creates live tenders. ApiKeyHeaderName/ApiKeyValue are a placeholder
        // shape - if the real mechanism turns out to be Bearer/OAuth/a session
        // cookie instead, ETendersAdminApiClient's BuildAuthHeader will need to
        // change, but the rest of the client (the multipart request itself)
        // shouldn't need to.
        public string? ApiKeyHeaderName { get; set; }
        public string? ApiKeyValue { get; set; }
    }
}
