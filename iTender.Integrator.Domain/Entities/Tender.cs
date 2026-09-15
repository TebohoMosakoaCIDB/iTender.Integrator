using iTender.Integrator.Domain.Common;
using iTender.Integrator.Domain.Enums;
using iTender.Integrator.Domain.ValueObjects;

namespace iTender.Integrator.Domain.Entities
{
    public sealed class Tender : Entity<Guid>
    {
        public string ExternalId { get; private set; } = default!;

        public string Title { get; private set; } = default!;

        public TenderStatus Status { get; private set; }

        public string? Category { get; private set; }

        public string? MainProcurementCategory { get; private set; }

        private readonly List<string> _additionalProcurementCategories = new();
        public IReadOnlyCollection<string> AdditionalProcurementCategories => _additionalProcurementCategories.AsReadOnly();

        public string? Description { get; private set; }

        public string? Province { get; private set; }

        public string? DeliveryLocation { get; private set; }

        public string? EligibilityCriteria { get; private set; }

        public string? ProcurementMethod { get; private set; }

        public string? ProcurementMethodDetails { get; private set; }

        public string? ProcuringEntityId { get; private set; }

        public string? ProcuringEntityName { get; private set; }

        public Classification? Classification { get; private set; }

        public Money? Value { get; private set; }

        public DateRange? TenderPeriod { get; private set; }

        public DateRange? EnquiryPeriod { get; private set; }

        public DateRange? AwardPeriod { get; private set; }

        public ContactPoint? ContactPerson { get; private set; }

        public BriefingSession? BriefingSession { get; private set; }

        private readonly List<Lot> _lots = new();
        public IReadOnlyCollection<Lot> Lots => _lots.AsReadOnly();

        private readonly List<TenderItem> _items = new();
        public IReadOnlyCollection<TenderItem> Items => _items.AsReadOnly();

        private readonly List<TenderDocument> _documents = new();
        public IReadOnlyCollection<TenderDocument> Documents => _documents.AsReadOnly();

        private Tender()
        {
        }

        public static Tender Create(
            string externalId,
            string title,
            TenderStatus status,
            string? category,
            string? mainProcurementCategory,
            string? description,
            Classification? classification,
            Money? value,
            DateRange? tenderPeriod)
        {
            if (string.IsNullOrWhiteSpace(externalId))
                throw new ArgumentException("Tender id is required.", nameof(externalId));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Tender title is required.", nameof(title));

            return new Tender
            {
                Id = Guid.NewGuid(),
                ExternalId = externalId,
                Title = title,
                Status = status,
                Category = category,
                MainProcurementCategory = mainProcurementCategory,
                Description = description,
                Classification = classification,
                Value = value,
                TenderPeriod = tenderPeriod
            };
        }

        public void SetLocation(string? province, string? deliveryLocation)
        {
            Province = province;
            DeliveryLocation = deliveryLocation;
        }

        public void SetProcuringEntity(string? id, string? name)
        {
            ProcuringEntityId = id;
            ProcuringEntityName = name;
        }

        public void SetProcurementMethod(string? method, string? details)
        {
            ProcurementMethod = method;
            ProcurementMethodDetails = details;
        }

        public void SetPeriods(DateRange? enquiryPeriod, DateRange? awardPeriod)
        {
            EnquiryPeriod = enquiryPeriod;
            AwardPeriod = awardPeriod;
        }

        public void SetEligibilityCriteria(string? criteria) => EligibilityCriteria = criteria;

        public void SetAdditionalProcurementCategories(IEnumerable<string>? categories)
        {
            _additionalProcurementCategories.Clear();
            if (categories is null) return;

            foreach (var category in categories)
                if (!string.IsNullOrWhiteSpace(category))
                    _additionalProcurementCategories.Add(category);
        }

        // OCDS's standard taxonomy for mainProcurementCategory is exactly three
        // values: "goods", "works", "services". cidb's mandate is construction, which
        // in that taxonomy is "works" - so this is a standards-based signal, not a
        // guessed keyword list. tender.Category (free text, e.g. "Education",
        // "Information service activities") is NOT used here since it varies by
        // procuring entity and isn't a reliable filter.
        public bool IsConstructionRelated =>
            string.Equals(MainProcurementCategory, "works", StringComparison.OrdinalIgnoreCase)
            || _additionalProcurementCategories.Any(c => string.Equals(c, "works", StringComparison.OrdinalIgnoreCase));

        public void SetContactPerson(ContactPoint? contactPerson) => ContactPerson = contactPerson;

        public void SetBriefingSession(BriefingSession? session) => BriefingSession = session;

        public void AddLot(Lot lot) => _lots.Add(lot ?? throw new ArgumentNullException(nameof(lot)));

        public void AddItem(TenderItem item) => _items.Add(item ?? throw new ArgumentNullException(nameof(item)));

        public void AddDocument(TenderDocument document) => _documents.Add(document ?? throw new ArgumentNullException(nameof(document)));

        public bool IsOpenForSubmission(DateTime asOfUtc)
            => Status == TenderStatus.Active && (TenderPeriod?.IsOpenNow(asOfUtc) ?? false);
    }
}