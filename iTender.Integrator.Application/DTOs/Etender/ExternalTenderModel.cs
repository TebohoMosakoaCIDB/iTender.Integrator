namespace iTender.Integrator.Application.DTOs.Etender
{
    public class ExternalTenderModel
    {
        public int Id { get; set; }
        public string? Tender_No { get; set; }
        public string? Type { get; set; }
        public string? Delivery { get; set; }
        public string? Department { get; set; }
        public DateTime? Date_Published { get; set; }
        public bool? Cbrief { get; set; }
        public string? Cd { get; set; }
        public string? Dp { get; set; }
        public DateTime? Closing_Date { get; set; }
        public string? Brief { get; set; }
        public DateTime? Compulsory_Briefing_Session { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? ProvinceId { get; set; }
        public string? Status { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public string? Province { get; set; }
        public string? ContactPerson { get; set; }
        public string? Email { get; set; }
        public string? Telephone { get; set; }
        public string? Fax { get; set; }
        public string? BriefingVenue { get; set; }
        public string? Conditions { get; set; }
        public object? SupportDocuments { get; set; }
        public string? Bf { get; set; }
        public object? BriefingSession { get; set; }
        public string? Bc { get; set; }
        public object? BriefingCompulsory { get; set; }
        public bool ESubmission { get; set; }
        public string? ClosingReason { get; set; }
        public string? CancelledReason { get; set; }
        public object? AwardedCompanies { get; set; }
        public object? Bidders { get; set; }
        public object? Awarded_Contact { get; set; }
        public string? CancellationReason { get; set; }
    }
}
