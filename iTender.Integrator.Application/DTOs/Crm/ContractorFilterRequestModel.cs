namespace iTender.Integrator.Application.DTOs.Crm
{
    public class ContractorFilterRequestModel
    {
        public string? ProvinceId { get; set; }
        public string? City { get; set; }
        public string? ContractorName { get; set; }
        public string? Status { get; set; }
        public string? ClassOfConstructionWorksId { get; set; }
        public string? CRSNumber { get; set; }
        public int DesignationId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
