using iTender.Integrator.Application.DTOs.Ocds;
using iTender.Integrator.Domain.Entities;
using iTender.Integrator.Domain.Enums;
using iTender.Integrator.Domain.ValueObjects;
using System.Globalization;

namespace iTender.Integrator.Application.Mapping
{
    public static class OcdsReleaseMapper
    {
        public static Release ToDomain(OcdsReleaseDto dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var release = Release.Create(
                ocid: dto.OcId ?? string.Empty,
                releaseId: dto.Id ?? string.Empty,
                releaseDate: ParseDate(dto.Date) ?? DateTime.UtcNow,
                description: dto.Description,
                initiationType: dto.InitiationType,
                language: dto.Language);

            foreach (var tag in dto.Tag)
                release.AddTag(tag);

            release.SetBuyer(dto.Buyer?.Id, dto.Buyer?.Name);

            foreach (var partyDto in dto.Parties)
                release.AddParty(MapParty(partyDto));

            if (dto.Tender is not null)
                release.AttachTender(MapTender(dto.Tender));

            foreach (var awardDto in dto.Awards)
                release.AddAward(MapAward(awardDto));

            foreach (var contractDto in dto.Contracts)
                release.AddContract(MapContract(contractDto));

            return release;
        }

        private static Tender MapTender(OcdsTenderDto dto)
        {
            var tender = Tender.Create(
                externalId: dto.Id ?? string.Empty,
                title: dto.Title ?? string.Empty,
                status: MapTenderStatus(dto.Status),
                category: dto.Category,
                mainProcurementCategory: dto.MainProcurementCategory,
                description: dto.Description,
                classification: MapClassification(dto.Classification),
                value: MapMoney(dto.Value),
                tenderPeriod: MapPeriod(dto.TenderPeriod));

            tender.SetLocation(dto.Province, dto.DeliveryLocation);
            tender.SetProcuringEntity(dto.ProcuringEntity?.Id, dto.ProcuringEntity?.Name);
            tender.SetProcurementMethod(dto.ProcurementMethod, dto.ProcurementMethodDetails);
            tender.SetPeriods(MapPeriod(dto.EnquiryPeriod), MapPeriod(dto.AwardPeriod));
            tender.SetEligibilityCriteria(dto.EligibilityCriteria);

            if (dto.ContactPerson is not null)
            {
                tender.SetContactPerson(ContactPoint.Create(
                    dto.ContactPerson.Name,
                    dto.ContactPerson.TelephoneNumber,
                    dto.ContactPerson.Email,
                    dto.ContactPerson.FaxNumber));
            }

            if (dto.BriefingSession is not null)
            {
                tender.SetBriefingSession(BriefingSession.Create(
                    dto.BriefingSession.IsSession,
                    dto.BriefingSession.Compulsory,
                    ParseDate(dto.BriefingSession.Date),
                    dto.BriefingSession.Venue));
            }

            foreach (var lotDto in dto.Lots)
                tender.AddLot(MapLot(lotDto));

            foreach (var itemDto in dto.Items)
                tender.AddItem(MapItem(itemDto));

            foreach (var documentDto in dto.Documents)
                tender.AddDocument(MapDocument(documentDto));

            return tender;
        }

        private static Lot MapLot(OcdsLotDto dto)
            => Lot.Create(
                externalId: dto.Id ?? string.Empty,
                description: dto.Description,
                value: MapMoney(dto.Value),
                contractPeriod: MapPeriod(dto.ContractPeriod),
                status: dto.Status,
                hasRenewal: dto.HasRenewal,
                hasOptions: dto.HasOptions);

        private static TenderItem MapItem(OcdsItemDto dto)
            => TenderItem.Create(
                externalId: dto.Id ?? string.Empty,
                description: dto.Description,
                classification: MapClassification(dto.Classifications),
                quantity: dto.Quantity ?? 0,
                unit: dto.Unit);

        private static TenderDocument MapDocument(OcdsDocumentDto dto)
            => TenderDocument.Create(
                externalId: dto.Id ?? string.Empty,
                url: dto.Url ?? string.Empty,
                documentType: dto.DocumentType,
                title: dto.Title,
                description: dto.Description,
                datePublished: ParseDate(dto.DatePublished),
                dateModified: ParseDate(dto.DateModified),
                format: dto.Format,
                language: dto.Language);

        private static Party MapParty(OcdsPartyDto dto)
        {
            var roles = PartyRole.None;
            foreach (var role in dto.Roles)
                roles |= MapPartyRole(role);

            Address? address = dto.Address is null
                ? null
                : Address.Create(dto.Address.StreetAddress, dto.Address.Locality, dto.Address.Region, dto.Address.PostalCode, dto.Address.CountryName);

            ContactPoint? contactPoint = dto.ContactPoint is null
                ? null
                : ContactPoint.Create(dto.ContactPoint.Name, dto.ContactPoint.Telephone, dto.ContactPoint.Email, dto.ContactPoint.FaxNumber, dto.ContactPoint.Url);

            return Party.Create(
                externalId: dto.Id ?? string.Empty,
                name: dto.Name ?? string.Empty,
                roles: roles,
                legalName: dto.Identifier?.LegalName,
                address: address,
                contactPoint: contactPoint);
        }

        private static Award MapAward(OcdsAwardDto dto)
        {
            var award = Award.Create(
                externalId: dto.Id ?? string.Empty,
                title: dto.Title,
                status: MapAwardStatus(dto.Status),
                description: dto.Description,
                value: MapMoney(dto.Value));

            foreach (var supplierDto in dto.Suppliers)
                award.AddSupplier(AwardSupplier.Create(supplierDto.Id ?? string.Empty, supplierDto.Name ?? string.Empty));

            return award;
        }

        private static Contract MapContract(OcdsContractDto dto)
        {
            var contract = Contract.Create(
                externalId: dto.Id ?? string.Empty,
                awardExternalId: dto.AwardID,
                title: dto.Title,
                description: dto.Description,
                status: MapContractStatus(dto.Status),
                period: MapPeriod(dto.Period),
                value: MapMoney(dto.Value),
                dateSigned: ParseDate(dto.DateSigned));

            foreach (var documentDto in dto.Documents)
                contract.AddDocument(MapDocument(documentDto));

            foreach (var milestoneDto in dto.Milestones)
                contract.AddMilestone(MapMilestone(milestoneDto));

            if (dto.Implementation is not null)
            {
                foreach (var milestoneDto in dto.Implementation.Milestones)
                    contract.AddMilestone(MapMilestone(milestoneDto));

                foreach (var transactionDto in dto.Implementation.Transactions)
                {
                    contract.AddTransaction(ContractTransaction.Create(
                        transactionDto.Id ?? string.Empty,
                        ParseDate(transactionDto.Date),
                        MapMoney(transactionDto.Value),
                        transactionDto.Payer?.Id,
                        transactionDto.Payee?.Id));
                }
            }

            return contract;
        }

        private static ContractMilestone MapMilestone(OcdsMilestoneDto dto)
            => ContractMilestone.Create(
                externalId: dto.Id ?? string.Empty,
                title: dto.Title,
                type: dto.Type,
                status: MapMilestoneStatus(dto.Status),
                dueDate: ParseDate(dto.DueDate),
                dateMet: ParseDate(dto.DateMet));

        private static Money? MapMoney(OcdsValueDto? dto)
            => dto is null ? null : Money.Create(dto.Amount, dto.Currency);

        private static Classification? MapClassification(OcdsClassificationDto? dto)
            => dto is null ? null : Classification.Create(dto.Scheme, dto.Id, dto.Description);

        private static DateRange? MapPeriod(OcdsPeriodDto? dto)
            => dto is null
                ? null
                : DateRange.Create(dto.StartDate, dto.EndDate, ParseDate(dto.MaxExtentDate), dto.DurationInDays);

        private static DateTime? ParseDate(string? value)
            => !string.IsNullOrWhiteSpace(value) && DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed)
                ? parsed
                : null;

        private static TenderStatus MapTenderStatus(string? status) => status?.Trim().ToLowerInvariant() switch
        {
            "planning" or "planned" => TenderStatus.Planning,
            "active" => TenderStatus.Active,
            "cancelled" => TenderStatus.Cancelled,
            "unsuccessful" => TenderStatus.Unsuccessful,
            "complete" => TenderStatus.Complete,
            "withdrawn" => TenderStatus.Withdrawn,
            _ => TenderStatus.Unknown
        };

        private static AwardStatus MapAwardStatus(string? status) => status?.Trim().ToLowerInvariant() switch
        {
            "pending" => AwardStatus.Pending,
            "active" => AwardStatus.Active,
            "cancelled" => AwardStatus.Cancelled,
            "unsuccessful" => AwardStatus.Unsuccessful,
            _ => AwardStatus.Unknown
        };

        private static ContractStatus MapContractStatus(string? status) => status?.Trim().ToLowerInvariant() switch
        {
            "pending" => ContractStatus.Pending,
            "active" => ContractStatus.Active,
            "cancelled" => ContractStatus.Cancelled,
            "terminated" => ContractStatus.Terminated,
            "complete" => ContractStatus.Complete,
            _ => ContractStatus.Unknown
        };

        private static MilestoneStatus MapMilestoneStatus(string? status) => status?.Trim().ToLowerInvariant() switch
        {
            "scheduled" => MilestoneStatus.Scheduled,
            "met" => MilestoneStatus.Met,
            "notmet" or "not_met" => MilestoneStatus.NotMet,
            "partiallymet" or "partially_met" => MilestoneStatus.PartiallyMet,
            _ => MilestoneStatus.Unknown
        };

        private static PartyRole MapPartyRole(string? role) => role?.Trim().ToLowerInvariant() switch
        {
            "buyer" => PartyRole.Buyer,
            "procuringentity" => PartyRole.ProcuringEntity,
            "supplier" => PartyRole.Supplier,
            "tenderer" => PartyRole.Tenderer,
            "payer" => PartyRole.Payer,
            "payee" => PartyRole.Payee,
            "reviewbody" => PartyRole.ReviewBody,
            "enquirer" => PartyRole.Enquirer,
            "funder" => PartyRole.Funder,
            _ => PartyRole.None
        };
    }
}
