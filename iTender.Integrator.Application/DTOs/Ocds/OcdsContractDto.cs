namespace iTender.Integrator.Application.DTOs.Ocds
{
    public class OcdsContractDto
    {
        public string? Id { get; set; }

        public string? AwardID { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Status { get; set; }

        public OcdsPeriodDto? Period { get; set; }

        public OcdsValueDto? Value { get; set; }

        public string? DateSigned { get; set; }

        public List<OcdsDocumentDto> Documents { get; set; } = [];

        public OcdsImplementationDto? Implementation { get; set; }

        public List<OcdsRelatedProcessDto> RelatedProcesses { get; set; } = [];

        public List<OcdsMilestoneDto> Milestones { get; set; } = [];
    }
}
