namespace iTender.Integrator.Infrastructure.Integrations.CRM
{
    public class EncryptionOptions
    {
        public const string SectionName = "Encryption";
        public required string Key { get; set; }
        public required string IV { get; set; }
    }
}
