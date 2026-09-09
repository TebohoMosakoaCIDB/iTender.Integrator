using iTender.Integrator.Domain.Common;
using iTender.Integrator.Domain.Enums;
using iTender.Integrator.Domain.ValueObjects;

namespace iTender.Integrator.Domain.Entities
{
    public sealed class Party : Entity<Guid>
    {
        public CidbComplianceStatus ComplianceStatus { get; private set; } = CidbComplianceStatus.NotChecked;

        public DateTime? LastComplianceCheckUtc { get; private set; }

        public string ExternalId { get; private set; } = default!;

        public string Name { get; private set; } = default!;

        public string? LegalName { get; private set; }

        // The register a RegistrationNumber belongs to, e.g. "ZA-CSD". Null when the
        // release didn't carry an identifier at all - compliance checking can't proceed
        // for that party without one.
        public string? RegistrationScheme { get; private set; }

        // The actual CSD supplier number (or other register id) - the join key into
        // ICsdApiClient and, via CRM's nv_csdnumber, into the CIDB contractor record.
        public string? RegistrationNumber { get; private set; }

        public Address? Address { get; private set; }

        public ContactPoint? ContactPoint { get; private set; }

        public PartyRole Roles { get; private set; }

        public Guid? ContractorId { get; private set; }

        private Party()
        {
        }

        public static Party Create(
            string externalId,
            string name,
            PartyRole roles,
            string? legalName = null,
            string? registrationScheme = null,
            string? registrationNumber = null,
            Address? address = null,
            ContactPoint? contactPoint = null)
        {
            if (string.IsNullOrWhiteSpace(externalId))
                throw new ArgumentException("Party id is required.", nameof(externalId));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Party name is required.", nameof(name));

            return new Party
            {
                Id = Guid.NewGuid(),
                ExternalId = externalId,
                Name = name,
                LegalName = legalName,
                RegistrationScheme = registrationScheme,
                RegistrationNumber = registrationNumber,
                Address = address,
                ContactPoint = contactPoint,
                Roles = roles
            };
        }

        // Whether this party carries enough of a registration identifier to even
        // attempt a CSD/CRM compliance lookup.
        public bool HasRegistrationIdentifier => !string.IsNullOrWhiteSpace(RegistrationNumber);

        public void AddRole(PartyRole role) => Roles |= role;

        public bool HasRole(PartyRole role) => (Roles & role) == role;

        public void LinkToContractor(Guid contractorId) => ContractorId = contractorId;

        public void SetComplianceStatus(CidbComplianceStatus status)
        {
            ComplianceStatus = status;
            LastComplianceCheckUtc = DateTime.UtcNow;
        }
    }
}
