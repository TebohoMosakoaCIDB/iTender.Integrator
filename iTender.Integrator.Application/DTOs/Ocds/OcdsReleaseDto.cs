namespace iTender.Integrator.Application.DTOs.Ocds
{
    public class OcdsReleaseDto
    {
        public string? OcId { get; set; }

        public string? Id { get; set; }

        public string? Date { get; set; }

        public List<string> Tag { get; set; } = [];

        public string? Description { get; set; }

        public string? InitiationType { get; set; }

        public OcdsTenderDto? Tender { get; set; }

        public OcdsPlanningDto? Planning { get; set; }

        public List<OcdsPartyDto> Parties { get; set; } = [];

        public OcdsPartyReferenceDto? Buyer { get; set; }

        public string? Language { get; set; }

        public List<OcdsAwardDto> Awards { get; set; } = [];

        public List<OcdsContractDto> Contracts { get; set; } = [];

        public List<OcdsRelatedProcessDto> RelatedProcesses { get; set; } = [];
    }
}
