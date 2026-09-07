namespace iTender.Integrator.Application.DTOs.Crm
{
    public class ContractorGradeModel
    {
        public Guid ClassOfWorkId { get; set; }
        public string? ClassOfWorksDescription { get; set; }
        public string? ApprovedGrade { get; set; }
        public string? StatusText { get; set; }
        public DateTime? DateOfRegistration { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string? ElectricalLicense { get; set; }
    }
}
