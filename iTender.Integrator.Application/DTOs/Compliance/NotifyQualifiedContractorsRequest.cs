namespace iTender.Integrator.Application.DTOs.Compliance
{
    public sealed class NotifyQualifiedContractorsRequest
    {
        public Guid? ProvinceId { get; set; }
        public string? RequiredGradingDesignationContains { get; set; }
        public required string Subject { get; set; }
        public required string Message { get; set; }
    }
}
