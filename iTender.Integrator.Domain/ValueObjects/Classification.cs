using iTender.Integrator.Domain.Common;

namespace iTender.Integrator.Domain.ValueObjects
{
    public sealed class Classification : ValueObject
    {
        public string Scheme { get; }

        public string Code { get; }

        public string? Description { get; }

        private Classification(string scheme, string code, string? description)
        {
            Scheme = scheme;
            Code = code;
            Description = description;
        }

        public static Classification Create(string? scheme, string? code, string? description = null)
            => new(scheme?.Trim() ?? string.Empty, code?.Trim() ?? string.Empty, description);

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Scheme;
            yield return Code;
        }
    }
}
