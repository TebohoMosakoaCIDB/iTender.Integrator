using iTender.Integrator.Domain.Common;

namespace iTender.Integrator.Domain.ValueObjects
{
    public sealed class ContractTransaction : ValueObject
    {
        public string ExternalId { get; }

        public DateTime? Date { get; }

        public Money? Value { get; }

        public string? PayerId { get; }

        public string? PayeeId { get; }

        // EF Core needs this: it can't bind an owned-type value (Money) as a
        // constructor parameter (see the "No suitable constructor was found" error
        // this fixes), so it falls back to a parameterless constructor plus
        // backing-field access for the get-only properties above. The public
        // surface (Create(...), no setters) is unchanged - this is purely for EF's
        // materialization path, application code still can't construct an invalid
        // ContractTransaction.
        private ContractTransaction()
        {
            ExternalId = string.Empty;
        }

        private ContractTransaction(string externalId, DateTime? date, Money? value, string? payerId, string? payeeId)
        {
            ExternalId = externalId;
            Date = date;
            Value = value;
            PayerId = payerId;
            PayeeId = payeeId;
        }

        public static ContractTransaction Create(string externalId, DateTime? date, Money? value, string? payerId, string? payeeId)
            => new(externalId, date, value, payerId, payeeId);

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return ExternalId;
            yield return Date;
            yield return Value;
            yield return PayerId;
            yield return PayeeId;
        }
    }
}
