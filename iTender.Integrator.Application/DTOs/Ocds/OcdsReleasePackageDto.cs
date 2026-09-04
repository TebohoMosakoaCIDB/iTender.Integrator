namespace iTender.Integrator.Application.DTOs.Ocds
{
    public class OcdsReleasePackageDto
    {
        public string? Uri { get; set; }

        public string? Version { get; set; }

        public string? PublishedDate { get; set; }

        public OcdsPublisherDto? Publisher { get; set; }

        public string? License { get; set; }

        public string? PublicationPolicy { get; set; }

        public List<OcdsReleaseDto> Releases { get; set; } = [];
    }
}
