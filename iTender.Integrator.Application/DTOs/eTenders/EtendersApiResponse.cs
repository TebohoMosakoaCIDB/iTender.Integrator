namespace iTender.Integrator.Application.DTOs.ETenders
{
    public sealed class EtendersApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public EtendersTenderResponse? Data { get; set; }
        public List<EtendersErrorDetail>? Errors { get; set; }
    }

    public sealed class EtendersTenderResponse
    {
        public int Id { get; set; }
        public string? TenderNo { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public DateTime? ClosingDate { get; set; }
    }

    public sealed class EtendersErrorDetail
    {
        public string? Field { get; set; }
        public string? Message { get; set; }
    }
}
