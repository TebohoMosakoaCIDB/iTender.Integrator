using iTender.Integrator.Application.DTOs.Crm;
using iTender.Integrator.Domain.Constants;
using iTender.Integrator.Domain.Entities;
using iTender.Integrator.Domain.Enums;
using Microsoft.Xrm.Sdk;
using static iTender.Integrator.Domain.Constants.CrmFieldNames;

namespace iTender.Integrator.Infrastructure.Mappers.Crm
{
    public static class TenderMapper
    {
        // Confirmed against a previous project's real StatusCodeId writes. Only these
        // three are known; TenderStatus.Unsuccessful/Complete/Withdrawn/Unknown are
        // deliberately left unmapped below rather than guessed - confirm the real
        // codes before adding them.
        private static readonly Dictionary<TenderStatus, int> KnownCrmStatusCodes = new()
        {
            [TenderStatus.Planning] = 1,          // Draft
            [TenderStatus.Active] = 100000000,    // Advertised
            [TenderStatus.Cancelled] = 100000001  // Cancelled
        };
        // EmployerTenderNumber = tender.Title, confirmed against a real release: OCDS
        // "title" here holds the employer's own quote/reference number (e.g.
        // "FQ70/2026"), while the actual descriptive tender title lives in
        // "description" (e.g. "PROCUREMENT OF DEVELOPMENT AND ANALYSIS TRAINING
        // COURSE."). CreateTenderModel.Title/Name are mapped from Description
        // accordingly - the reverse of what this used to do.
        public static CreateTenderModel ToCreateTenderModel(Release release, Guid? provinceId = null)
        {
            if (release is null) throw new ArgumentNullException(nameof(release));
            if (release.Tender is null)
                throw new InvalidOperationException(
                    $"Release {release.Ocid}/{release.ReleaseId} has no tender attached - nothing to publish.");

            var tender = release.Tender;

            var model = new CreateTenderModel
            {
                EmployerTenderNumber = tender.Title,
                Title = tender.Description ?? tender.Title,
                Name = tender.Description ?? tender.Title,
                TendersInvitedFor = tender.Category,
                EligibilityCriteria = tender.EligibilityCriteria,
                Status = tender.Status,
                ClosingDateTime = tender.TenderPeriod?.EndDate
                    ?? throw new InvalidOperationException(
                        $"Tender {tender.ExternalId} has no tenderPeriod.endDate - required to publish to CRM."),
                DocumentsAvailableFrom = tender.TenderPeriod?.StartDate,
                PrimaryAddress = new AddressModel
                {
                    Line1 = tender.DeliveryLocation,
                    Province = tender.Province
                },
                ProvinceId = provinceId
            };

            if (tender.ContactPerson is not null)
            {
                model.ContactPerson.Add(new ContactForTenderModel
                {
                    PersonToQuery = tender.ContactPerson.Name ?? string.Empty,
                    MobilePhoneNumber = tender.ContactPerson.Telephone ?? string.Empty,
                    TelephoneNumber = tender.ContactPerson.Telephone,
                    FaxNumber = tender.ContactPerson.FaxNumber,
                    Email = tender.ContactPerson.Email
                });
            }

            if (tender.BriefingSession is { IsSession: true } briefing)
            {
                model.ClarificationMeetingRequired = 100000000; // CRM boolean-style option set - "Yes"
                model.ClarificationMeetingCompulsory = briefing.Compulsory ? 100000000 : 100000001;
                model.ClarificationMeetingPlace = briefing.Venue;
                model.ClarificationMeetingDateAndTime = briefing.Date;
            }

            // Deliberately left null: ProvinceId, MetroDistrictId, LocalMunicipalityId,
            // ClassOfConstructionWorksId, SubCategoryId, TenderValueRangeId,
            // EmployerId. These are all Dataverse lookup references (Guids into
            // nv_province / nv_classofworktype / etc), and OCDS gives us free-text
            // (tender.Province, tender.Category) rather than those record ids. Writing
            // a wrong guess here is worse than leaving it blank - needs a name-to-id
            // lookup against those reference entities, which isn't built yet.

            return model;
        }

        public static Entity ToEntity(CreateTenderModel model, Guid? existingId = null)
        {
            var entity = existingId.HasValue
                ? new Entity(CrmEntityNames.Tender, existingId.Value)
                : new Entity(CrmEntityNames.Tender);

            entity[TenderFields.EmployerTenderNumber] = model.EmployerTenderNumber;
            entity[TenderFields.Title] = model.Title;
            entity[TenderFields.Name] = model.Name;
            entity[TenderFields.TendersInvitedFor] = model.TendersInvitedFor;
            entity[TenderFields.EligibilityCriteria] = model.EligibilityCriteria;
            entity[TenderFields.ClosingDate] = model.ClosingDateTime;

            if (model.DocumentsAvailableFrom.HasValue)
                entity[TenderFields.DocsAvailableFrom] = model.DocumentsAvailableFrom.Value;

            if (model.ProvinceId.HasValue)
                entity[TenderFields.ProvinceId] = new EntityReference(CrmEntityNames.Province, model.ProvinceId.Value);

            if (model.Status.HasValue)
            {
                if (KnownCrmStatusCodes.TryGetValue(model.Status.Value, out var statusCode))
                {
                    entity[TenderFields.StatusCode] = new OptionSetValue(statusCode);

                    // Side effects confirmed against the same previous-project code
                    // the status codes came from - Advertised stamps DateAdvertised,
                    // Cancelled flags IsClosed.
                    if (model.Status.Value == TenderStatus.Active)
                        entity[TenderFields.DateAdvertised] = DateTime.UtcNow;

                    if (model.Status.Value == TenderStatus.Cancelled)
                        entity[TenderFields.IsClosed] = true;
                }
                // else: TenderStatus.Unsuccessful/Complete/Withdrawn/Unknown - no
                // confirmed CRM code yet, so statuscode (and any side effects) are
                // left untouched rather than guessed.
            }

            if (!string.IsNullOrWhiteSpace(model.PrimaryAddress?.Line1))
                entity[TenderFields.PrimaryAddressLine1] = model.PrimaryAddress!.Line1;

            if (model.ClarificationMeetingPlace is not null)
            {
                // ClarificationMeetingRequired/Compulsory are Dataverse choice
                // (OptionSetValue) fields, not plain integers - the SDK will reject
                // a raw int here. The 100000000/100000001 = Yes/No convention matches
                // how CreateTenderModel's other option fields are defaulted, but
                // hasn't been confirmed against the live nv_tender choice definitions.
                entity[TenderFields.ClarificationMeeting] = new OptionSetValue(model.ClarificationMeetingRequired);
                entity[TenderFields.ClarificationMeetingPlace] = model.ClarificationMeetingPlace;

                if (model.ClarificationMeetingDateAndTime.HasValue)
                    entity[TenderFields.ClarificationMeetingDateAndTime] = model.ClarificationMeetingDateAndTime.Value;

                if (model.ClarificationMeetingCompulsory.HasValue)
                    entity[TenderFields.ClarificationMeetingCompulsory] =
                        new OptionSetValue(model.ClarificationMeetingCompulsory.Value);
            }

            return entity;
        }
    }
}