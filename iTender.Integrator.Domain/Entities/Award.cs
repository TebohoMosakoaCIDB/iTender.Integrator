using iTender.Integrator.Domain.Common;
using iTender.Integrator.Domain.Enums;
using iTender.Integrator.Domain.ValueObjects;

namespace iTender.Integrator.Domain.Entities
{
    public sealed class Award : Entity<Guid>
    {
        public string ExternalId { get; private set; } = default!;

        public string? Title { get; private set; }

        public AwardStatus Status { get; private set; }

        public string? Description { get; private set; }

        public Money? Value { get; private set; }

        private readonly List<AwardSupplier> _suppliers = new();
        public IReadOnlyCollection<AwardSupplier> Suppliers => _suppliers.AsReadOnly();

        private Award()
        {
        }

        public static Award Create(string externalId, string? title, AwardStatus status, string? description, Money? value)
        {
            if (string.IsNullOrWhiteSpace(externalId))
                throw new ArgumentException("Award id is required.", nameof(externalId));

            return new Award
            {
                Id = Guid.NewGuid(),
                ExternalId = externalId,
                Title = title,
                Status = status,
                Description = description,
                Value = value
            };
        }

        public void AddSupplier(AwardSupplier supplier) => _suppliers.Add(supplier ?? throw new ArgumentNullException(nameof(supplier)));
    }
    
}
