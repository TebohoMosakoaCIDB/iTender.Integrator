namespace iTender.Integrator.Application.DTOs.ETenders
{
    public enum EtendersTenderStatus
    {
        Published,
        Closed,
        Draft
    }

    public enum EtendersProcurementPlan
    {
        LinkNow,
        LinkLater
    }

    /// <summary>
    /// Mirrors POST /api/Etenders/add on admin-uat.etenders.gov.za exactly, per the
    /// real OpenAPI spec - field names, required-ness, and types all match. This is
    /// a multipart/form-data request (not JSON) because Documents is a file upload
    /// array, so ETendersAdminApiClient builds a MultipartFormDataContent from this
    /// rather than serializing it as JSON.
    /// </summary>
    public sealed class CreateTenderRequest
    {
        // ----- Required per the spec -----
        public required string Username { get; set; }
        public required string TenderNumber { get; set; }
        public required DateTime PublishedDate { get; set; }
        public required DateTime ClosingDate { get; set; }
        public required EtendersTenderStatus TenderStatus { get; set; }
        public required int TenderCategoryId { get; set; }
        public required int CategoriesID { get; set; }
        public required int OrganOfStateId { get; set; }
        public required int ProvinceId { get; set; }
        public required EtendersProcurementPlan ProcurementPlan { get; set; }
        public required string Description { get; set; }
        public required int TenderTypeId { get; set; }
        public required DateTime ValidityStartDate { get; set; }
        public required DateTime ValidityEndDate { get; set; }
        public required bool BriefingSession { get; set; }
        public required bool IsBriefingSessionCompulsory { get; set; }
        public required string StreetName { get; set; }
        public required string Surburb { get; set; } // sic - matches the real field name
        public required string Town { get; set; }
        public required string Code { get; set; }
        public required string ContactPerson { get; set; }
        public required string Email { get; set; }
        public required string Telephone { get; set; }
        public required bool ESubmission { get; set; }

        // ----- Optional per the spec -----
        public int? ProcurementId { get; set; }
        public int? TenderSubTypeId { get; set; }
        public string? BriefingVenue { get; set; }
        public DateTime? BriefingSessionDate { get; set; }
        public string? SpecialConditions { get; set; }
        public bool? CompanyProfile { get; set; }
        public bool? IsCompanyProfileMandatory { get; set; }
        public bool? ProposalDescriptionOfSolution { get; set; }
        public bool? IsProposalDescriptionOfSolutionMandatory { get; set; }
        public bool? HighLevelExecutionPlan { get; set; }
        public bool? IsHighLevelExecutionPlanMandatory { get; set; }
        public bool? PricingSchedule { get; set; }
        public bool? IsPricingScheduleMandatory { get; set; }
        public bool? BillingSchedule { get; set; }
        public bool? IsBillingScheduleMandatory { get; set; }
        public bool? Quote { get; set; }
        public bool? IsQuoteMandatory { get; set; }
        public bool? PricingToBeSubmittedInASeparateSealedEnvelope { get; set; }
        public List<string>? Questions { get; set; }
        public List<string>? Answer { get; set; }

        /// <summary>
        /// NOT part of the eTenders schema - this is our own addition, since eTenders
        /// has no concept of "is this construction" and we need one to decide
        /// whether to auto-forward to CRM. See ETendersController for why this is
        /// required from the caller rather than inferred from TenderCategoryId/
        /// CategoriesID (we don't have eTenders' own category taxonomy mapped to
        /// "is construction" yet).
        /// </summary>
        public required bool IsConstructionTender { get; set; }
    }

    public sealed record TenderDocumentUpload(string FileName, string ContentType, Stream Content);
}