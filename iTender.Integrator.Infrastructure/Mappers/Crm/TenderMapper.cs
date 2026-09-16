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
        // Confirmed: nv_tender.statuscode genuinely only has these four values -
        // there is no separate "Awarded" status. CRM tracks post-award lifecycle via
        // a separate Contract entity (nv_contract, which has its own TenderId lookup
        // back to this record) instead of a tender statuscode value. So this mapping
        // is now complete, not partial: every TenderStatus has a home.
        // Unsuccessful/Complete/Withdrawn all land on Closed - CRM has no finer-
        // grained distinction between "the tender process ended without an award"
        // and "the tender process ended because it was awarded" at the tender level;
        // that distinction lives in whether a linked Contract exists, not in this
        // field. TenderStatus.Unknown is deliberately left unmapped - an OCDS status
        // we couldn't even parse shouldn't be forced into a specific CRM state.
        private static readonly Dictionary<TenderStatus, iTenderTenderStatus> KnownCrmStatusCodes = new()
        {
            [TenderStatus.Planning] = iTenderTenderStatus.DRAFT_STATUS,
            [TenderStatus.Active] = iTenderTenderStatus.ADVERTISED_STATUS,
            [TenderStatus.Cancelled] = iTenderTenderStatus.CANCELLED_STATUS,
            [TenderStatus.Unsuccessful] = iTenderTenderStatus.CLOSED_STATUS,
            [TenderStatus.Complete] = iTenderTenderStatus.CLOSED_STATUS,
            [TenderStatus.Withdrawn] = iTenderTenderStatus.CLOSED_STATUS
        };
        // EmployerTenderNumber = tender.Title, confirmed against a real release: OCDS
        // "title" here holds the employer's own quote/reference number (e.g.
        // "FQ70/2026"), while the actual descriptive tender title lives in
        // "description" (e.g. "PROCUREMENT OF DEVELOPMENT AND ANALYSIS TRAINING
        // COURSE."). CreateTenderModel.Title/Name are mapped from Description
        // accordingly - the reverse of what this used to do.
        public static CreateTenderModel ToCreateTenderModel(
            Release release, Guid? provinceId = null, Guid? metroDistrictId = null, Guid? classOfWorkTypeId = null)
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
                ProvinceId = provinceId,
                MetroDistrictId = metroDistrictId,
                ClassOfConstructionWorksId = classOfWorkTypeId
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

            // Deliberately left null: LocalMunicipalityId, SubCategoryId,
            // TenderValueRangeId, EmployerId. ProvinceId, MetroDistrictId and
            // ClassOfConstructionWorksId are now resolved by the caller
            // (ReleaseComplianceService) and passed in above - the class-of-work
            // resolution is best-effort (name-in-text matching against CRM's own
            // nv_classofworktype names), same caveat as MetroDistrictId. The rest
            // still have no signal to resolve from at all.

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

            if (model.MetroDistrictId.HasValue)
                entity[TenderFields.MetroDistrictId] =
                    new EntityReference(CrmEntityNames.MetroDistrict, model.MetroDistrictId.Value);

            // Field name is nv_classofconstructionworkid - note it does NOT match the
            // nv_classofworktype entity name it points to (that's just how this was
            // named in CRM).
            if (model.ClassOfConstructionWorksId.HasValue)
                entity[TenderFields.ClassOfWork] =
                    new EntityReference(CrmEntityNames.ClassOfWorkType, model.ClassOfConstructionWorksId.Value);

            if (model.Status.HasValue)
            {
                if (KnownCrmStatusCodes.TryGetValue(model.Status.Value, out var crmStatus))
                {
                    entity[TenderFields.StatusCode] = new OptionSetValue((int)crmStatus);

                    // Side effects confirmed against the same previous-project code
                    // the status codes came from - Advertised stamps DateAdvertised.
                    // IsClosed is set for every status that lands on Closed
                    // (Cancelled, and now Unsuccessful/Complete/Withdrawn too) - the
                    // field name is generic, not Cancelled-specific.
                    if (model.Status.Value == TenderStatus.Active)
                        entity[TenderFields.DateAdvertised] = DateTime.UtcNow;

                    if (crmStatus is iTenderTenderStatus.CANCELLED_STATUS or iTenderTenderStatus.CLOSED_STATUS)
                        entity[TenderFields.IsClosed] = true;
                }
                // else: TenderStatus.Unknown - deliberately left unmapped, see the
                // comment on KnownCrmStatusCodes above.
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