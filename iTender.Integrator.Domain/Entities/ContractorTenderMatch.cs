using iTender.Integrator.Domain.Common;

namespace iTender.Integrator.Domain.Entities
{
    public sealed class ContractorTenderMatch : Entity<Guid>
    {
        public Guid ContractorId { get; private set; }

        public string Ocid { get; private set; } = default!;

        public string TenderExternalId { get; private set; } = default!;

        public bool GradingMatches { get; private set; }

        public bool ComplianceMatches { get; private set; }

        public DateTime MatchedAtUtc { get; private set; }

        public bool NotificationSent { get; private set; }

        private ContractorTenderMatch()
        {
        }

        public static ContractorTenderMatch Create(Guid contractorId, string ocid, string tenderExternalId, bool gradingMatches, bool complianceMatches)
        {
            if (string.IsNullOrWhiteSpace(ocid))
                throw new ArgumentException("OCID is required.", nameof(ocid));

            if (string.IsNullOrWhiteSpace(tenderExternalId))
                throw new ArgumentException("Tender id is required.", nameof(tenderExternalId));

            return new ContractorTenderMatch
            {
                Id = Guid.NewGuid(),
                ContractorId = contractorId,
                Ocid = ocid,
                TenderExternalId = tenderExternalId,
                GradingMatches = gradingMatches,
                ComplianceMatches = complianceMatches,
                MatchedAtUtc = DateTime.UtcNow
            };
        }

        public bool IsEligibleMatch => GradingMatches && ComplianceMatches;

        public void MarkNotified() => NotificationSent = true;
    }
}
