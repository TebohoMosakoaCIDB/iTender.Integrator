namespace iTender.Integrator.Api.Authentication
{
    public class ApiKeyOptions
    {
        public const string SectionName = "ApiKeyAuth";

        /// <summary>
        /// Add one entry per caller (e.g. a scheduler, a partner system, a
        /// developer testing locally) so a compromised or retired key can be
        /// removed without affecting anyone else. Config shape:
        /// "ApiKeyAuth": { "Keys": [ { "Key": "...", "Label": "..." } ] }
        /// Not written to appsettings.json here - add real keys wherever secrets
        /// are actually kept for this environment (user secrets, Key Vault,
        /// environment variables), not committed to source control.
        /// </summary>
        public List<ApiKeyEntry> Keys { get; set; } = new();
    }

    public class ApiKeyEntry
    {
        public string Key { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }
}
