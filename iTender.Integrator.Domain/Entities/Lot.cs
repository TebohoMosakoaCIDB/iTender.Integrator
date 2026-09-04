using iTender.Integrator.Domain.Common;
using iTender.Integrator.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace iTender.Integrator.Domain.Entities
{
    public sealed class Lot : Entity<Guid>
    {
        public string ExternalId { get; private set; } = default!;

        public string? Description { get; private set; }

        public Money? Value { get; private set; }

        public DateRange? ContractPeriod { get; private set; }

        public string? Status { get; private set; }

        public bool HasRenewal { get; private set; }

        public bool HasOptions { get; private set; }

        private Lot()
        {
        }

        public static Lot Create(string externalId, string? description, Money? value, DateRange? contractPeriod, string? status, bool hasRenewal = false, bool hasOptions = false)
        {
            if (string.IsNullOrWhiteSpace(externalId))
                throw new ArgumentException("Lot id is required.", nameof(externalId));

            return new Lot
            {
                Id = Guid.NewGuid(),
                ExternalId = externalId,
                Description = description,
                Value = value,
                ContractPeriod = contractPeriod,
                Status = status,
                HasRenewal = hasRenewal,
                HasOptions = hasOptions
            };
        }
    }
}

