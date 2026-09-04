using iTender.Integrator.Domain.Common;

namespace iTender.Integrator.Domain.ValueObjects
{
    public sealed class AwardSupplier : ValueObject
    {
        public string ExternalId { get; }

        public string Name { get; }

        private AwardSupplier(string externalId, string name)
        {
            ExternalId = externalId;
            Name = name;
        }

        public static AwardSupplier Create(string externalId, string name) => new(externalId, name);

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return ExternalId;
            yield return Name;
        }
    }
}
