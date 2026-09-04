using iTender.Integrator.Domain.Common;

namespace iTender.Integrator.Domain.ValueObjects
{
    public sealed class Money : ValueObject
    {
        public decimal Amount { get; }

        public string Currency { get; }

        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        // Defaults to ZAR when OCDS omits currency, which the eTenders feed sometimes does.
        public static Money Create(decimal amount, string? currency)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Monetary amount cannot be negative.");

            return new Money(amount, string.IsNullOrWhiteSpace(currency) ? "ZAR" : currency.Trim().ToUpperInvariant());
        }

        public static Money? CreateOrNull(decimal? amount, string? currency)
            => amount.HasValue ? Create(amount.Value, currency) : null;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}
