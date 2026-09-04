using iTender.Integrator.Domain.Common;
using iTender.Integrator.Domain.ValueObjects;

namespace iTender.Integrator.Domain.Entities
{
    public sealed class TenderItem : Entity<Guid>
    {
        public string ExternalId { get; private set; } = default!;

        public string? Description { get; private set; }

        public Classification? Classification { get; private set; }

        public decimal Quantity { get; private set; }

        public string? Unit { get; private set; }

        private TenderItem()
        {
        }

        public static TenderItem Create(string externalId, string? description, Classification? classification, decimal quantity, string? unit)
        {
            if (string.IsNullOrWhiteSpace(externalId))
                throw new ArgumentException("Item id is required.", nameof(externalId));

            return new TenderItem
            {
                Id = Guid.NewGuid(),
                ExternalId = externalId,
                Description = description,
                Classification = classification,
                Quantity = quantity,
                Unit = unit
            };
        }
    }
}
