using iTender.Integrator.Domain.Common;
using iTender.Integrator.Domain.Enums;
using iTender.Integrator.Domain.Events;
using System.Diagnostics.Contracts;

namespace iTender.Integrator.Domain.Entities
{
    public sealed class Release : AggregateRoot<Guid>
    {
        public string Ocid { get; private set; } = default!;

        public string ReleaseId { get; private set; } = default!;

        public DateTime ReleaseDate { get; private set; }

        public string? Description { get; private set; }

        public string? InitiationType { get; private set; }

        public string Language { get; private set; } = "en";

        private readonly List<string> _tags = new();
        public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

        public Tender? Tender { get; private set; }

        public string? BuyerId { get; private set; }

        public string? BuyerName { get; private set; }

        private readonly List<Party> _parties = new();
        public IReadOnlyCollection<Party> Parties => _parties.AsReadOnly();

        private readonly List<Award> _awards = new();
        public IReadOnlyCollection<Award> Awards => _awards.AsReadOnly();

        private readonly List<Contract> _contracts = new();
        public IReadOnlyCollection<Contract> Contracts => _contracts.AsReadOnly();

        public DateTime FetchedAtUtc { get; private set; }

        public DateTime? LastSyncedAtUtc { get; private set; }

        private Release()
        {
        }

        public static Release Create(string ocid, string releaseId, DateTime releaseDate, string? description, string? initiationType, string? language = "en")
        {
            if (string.IsNullOrWhiteSpace(ocid))
                throw new ArgumentException("OCID is required.", nameof(ocid));

            if (string.IsNullOrWhiteSpace(releaseId))
                throw new ArgumentException("Release id is required.", nameof(releaseId));

            return new Release
            {
                Id = Guid.NewGuid(),
                Ocid = ocid,
                ReleaseId = releaseId,
                ReleaseDate = releaseDate,
                Description = description,
                InitiationType = initiationType,
                Language = string.IsNullOrWhiteSpace(language) ? "en" : language,
                FetchedAtUtc = DateTime.UtcNow
            };
        }

        public void AttachTender(Tender tender)
        {
            Tender = tender ?? throw new ArgumentNullException(nameof(tender));
            Raise(new TenderPublishedDomainEvent(Ocid, tender.Id, tender.Title, DateTime.UtcNow));
        }

        public void AddTag(string? tag)
        {
            if (!string.IsNullOrWhiteSpace(tag) && !_tags.Contains(tag))
                _tags.Add(tag);
        }

        public void SetBuyer(string? buyerId, string? buyerName)
        {
            BuyerId = buyerId;
            BuyerName = buyerName;
        }

        public void AddParty(Party party)
        {
            if (party is null) throw new ArgumentNullException(nameof(party));
            if (_parties.Any(p => p.ExternalId == party.ExternalId)) return; // idempotent on re-sync
            _parties.Add(party);
        }

        public void AddAward(Award award)
        {
            if (award is null) throw new ArgumentNullException(nameof(award));
            _awards.Add(award);

            if (award.Status == AwardStatus.Active)
                Raise(new TenderAwardedDomainEvent(Ocid, award.Id, award.Value, DateTime.UtcNow));
        }

        public void AddContract(Contract contract)
            => _contracts.Add(contract ?? throw new ArgumentNullException(nameof(contract)));

        public void MarkSynced() => LastSyncedAtUtc = DateTime.UtcNow;

        // Point #3 - candidates for CIDB contractor matching.
        public IEnumerable<Party> GetContractorCandidateParties()
            => _parties.Where(p => p.HasRole(PartyRole.Supplier) || p.HasRole(PartyRole.Tenderer));
    }
}
