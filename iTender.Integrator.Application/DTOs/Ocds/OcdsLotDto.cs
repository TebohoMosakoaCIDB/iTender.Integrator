namespace iTender.Integrator.Application.DTOs.Ocds
{
    public class OcdsLotDto
    {
        public string? Id { get; set; }

        public string? Description { get; set; }

        public OcdsAwardCriteriaDto? AwardCriteria { get; set; }

        public OcdsValueDto? Value { get; set; }

        public OcdsPeriodDto? ContractPeriod { get; set; }

        public bool HasRenewal { get; set; }

        public OcdsRenewalDto? Renewal { get; set; }

        public OcdsSubmissionTermsDto? SubmissionTerms { get; set; }

        public bool HasOptions { get; set; }

        public OcdsOptionsDto? Options { get; set; }

        public string? Status { get; set; }
    }
}
