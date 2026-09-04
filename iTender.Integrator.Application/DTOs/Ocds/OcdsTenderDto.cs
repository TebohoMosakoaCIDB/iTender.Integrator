namespace iTender.Integrator.Application.DTOs.Ocds
{
    public class OcdsTenderDto
    {
        public string? Id { get; set; }

        public string? Title { get; set; }

        public string? Status { get; set; }

        public string? Category { get; set; }

        public string? Province { get; set; }

        public string? DeliveryLocation { get; set; }

        public string? SpecialConditions { get; set; }

        public string? MainProcurementCategory { get; set; }

        public List<string> AdditionalProcurementCategories { get; set; } = [];

        public string? Description { get; set; }

        public string? ReviewDetails { get; set; }

        public bool HasEnquiries { get; set; }

        public string? EligibilityCriteria { get; set; }

        public List<string> SubmissionMethod { get; set; } = [];

        public string? SubmissionMethodDetails { get; set; }

        public OcdsClassificationDto? Classification { get; set; }

        public OcdsValueDto? Value { get; set; }

        public List<OcdsLotDto> Lots { get; set; } = [];

        public List<OcdsItemDto> Items { get; set; } = [];

        public OcdsCommunicationDto? Communication { get; set; }

        public OcdsSelectionCriteriaDto? SelectionCriteria { get; set; }

        public List<OcdsDocumentDto> Documents { get; set; } = [];

        public OcdsOtherRequirementsDto? OtherRequirements { get; set; }

        public OcdsContractTermsDto? ContractTerms { get; set; }

        public OcdsTechniquesDto? Techniques { get; set; }

        public List<string> CoveredBy { get; set; } = [];

        public OcdsPeriodDto? AwardPeriod { get; set; }

        public OcdsPeriodDto? TenderPeriod { get; set; }

        public OcdsPeriodDto? EnquiryPeriod { get; set; }

        public OcdsLegalBasisDto? LegalBasis { get; set; }

        public OcdsPeriodDto? ContractPeriod { get; set; }

        public List<OcdsTendererDto> Tenderers { get; set; } = [];

        public OcdsPartyReferenceDto? ProcuringEntity { get; set; }

        public string? ProcurementMethod { get; set; }

        public string? ProcurementMethodDetails { get; set; }

        public OcdsBriefingSessionDto? BriefingSession { get; set; }

        public OcdsContactPersonDto? ContactPerson { get; set; }
    }
}
