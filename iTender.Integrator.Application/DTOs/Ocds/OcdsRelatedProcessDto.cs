namespace iTender.Integrator.Application.DTOs.Ocds
{
    public class OcdsRelatedProcessDto
    {
        public string? Id { get; set; }

        public List<string> Relationship { get; set; } = [];

        public string? Title { get; set; }

        public string? Scheme { get; set; }

        public OcdsIdentifierDto? Identifier { get; set; }

        public string? Uri { get; set; }
    }
}
