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
