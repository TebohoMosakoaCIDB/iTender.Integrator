namespace iTender.Integrator.Application.DTOs.Ocds
{
    public class OcdsItemDto
    {
        public string? Id { get; set; }

        public string? Description { get; set; }

        public string? Classification { get; set; }

        public OcdsClassificationDto? Classifications { get; set; }

        public decimal? Quantity { get; set; }

        public string? Unit { get; set; }

        public string? ItemId { get; set; }
    }
}
