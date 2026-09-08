using iTender.Integrator.Domain.Enums;

namespace iTender.Integrator.Application.DTOs.Crm
{
    public class CreateTenderModel
    {
        public string? EmployerTenderNumber { get; set; }
        public string? Title { get; set; }
        public string? Name { get; set; }
        public string? TendersInvitedFor { get; set; }
        public string? PreferencesOffered { get; set; }
        public string? EligibilityCriteria { get; set; }

        public Guid? ProvinceId { get; set; }
        public Guid? MetroDistrictId { get; set; }
        public Guid? LocalMunicipalityId { get; set; }

        public int? TypeOfContractId { get; set; } = 100000000;
        public Guid? ClassOfConstructionWorksId { get; set; }
        public Guid? SubCategoryId { get; set; }
        public Guid? AlternateClassOfConstructionWorksId { get; set; }
        public int? TenderValueRangeId { get; set; }
        public int? EmergingEnterpriseSupportId { get; set; } = 100000000;
        public string? NameOfTargetedDevelopmentProgramme { get; set; }
        public int NationalContractorDevelopmentProgrammeId { get; set; } = 100000000;
        public int? IsTermContract { get; set; } = 100000000;
        public TenderStatus? Status { get; set; }

        public Guid? EmployerId { get; set; }
        public AddressModel PrimaryAddress { get; set; } = new();
        public AddressModel? AdditionalCollectionAddress { get; set; }
        public DateTime? DocumentsAvailableFrom { get; set; }
        public decimal? DepositAmount { get; set; }
        public bool? MethodOfPaymentBankGuaranteedCheque { get; set; } = false;
        public bool? MethodOfPaymentCash { get; set; } = false;
        public bool? MethodOfPaymentProofOfDepost { get; set; } = false;
        public string? FurtherPaymentAndCollectionInformation { get; set; }

        public List<ContactForTenderModel> ContactPerson { get; set; } = new();

        public int ClarificationMeetingRequired { get; set; } = 100000000;
        public string? ClarificationMeetingPlace { get; set; }
        public DateTime? ClarificationMeetingDateAndTime { get; set; }
        public int? ClarificationMeetingCompulsory { get; set; } = 100000000;
        public int? AdditionalClarificationMeeting { get; set; } = 100000000;
        public string? AddClarificationMeetingPlace { get; set; }
        public DateTime? AddClarificationMeetingDateAndTime { get; set; }
        public int? AddClarificationMeetingCompulsory { get; set; } = 100000000;

        public DateTime ClosingDateTime { get; set; }
        public bool NotAcceptedEmail { get; set; } = false;
    }

    public class ContactForTenderModel
    {
        public string PersonToQuery { get; set; } = string.Empty;
        public string MobilePhoneNumber { get; set; } = string.Empty;
        public string? TelephoneNumber { get; set; }
        public string? FaxNumber { get; set; }
        public string? Email { get; set; }
    }
}
