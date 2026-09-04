using iTender.Integrator.Domain.Common;
using iTender.Integrator.Domain.Enums;
using iTender.Integrator.Domain.ValueObjects;

namespace iTender.Integrator.Domain.Entities
{
    public sealed class Party : Entity<Guid>
    {
        public string ExternalId { get; private set; } = default!;

        public string Name { get; private set; } = default!;

        public string? LegalName { get; private set; }

        public Address? Address { get; private set; }

        public ContactPoint? ContactPoint { get; private set; }

        public PartyRole Roles { get; private set; }

        public Guid? ContractorId { get; private set; }

        private Party()
        {
        }

        public static Party Create(string externalId, string name, PartyRole roles, string? legalName = null, Address? address = null, ContactPoint? contactPoint = null)
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
                Address = address,
                ContactPoint = contactPoint,
                Roles = roles
            };
        }

        public void AddRole(PartyRole role) => Roles |= role;

        public bool HasRole(PartyRole role) => (Roles & role) == role;

        public void LinkToContractor(Guid contractorId) => ContractorId = contractorId;
    }
}
