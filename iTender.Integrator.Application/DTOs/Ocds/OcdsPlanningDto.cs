namespace iTender.Integrator.Application.DTOs.Ocds
{
    public class OcdsPlanningDto
    {
        public string? Rationale { get; set; }

        public OcdsBudgetDto? Budget { get; set; }

        public List<OcdsDocumentDto> Documents { get; set; } = [];
    }
}
