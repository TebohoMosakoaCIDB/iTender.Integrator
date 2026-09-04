namespace iTender.Integrator.Application.DTOs.Ocds
{
    public class OcdsAwardDto
    {
        public string? Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Status { get; set; }

        public DateTime? Date { get; set; }

        public OcdsValueDto? Value { get; set; }

        public List<OcdsPartyDto> Suppliers { get; set; } = [];
    }
}
