using iTender.Integrator.Domain.Common;

namespace iTender.Integrator.Domain.ValueObjects
{
    public sealed class ContactPoint : ValueObject
    {
        public string? Name { get; }

        public string? Telephone { get; }

        public string? Email { get; }

        public string? FaxNumber { get; }

        public string? Url { get; }

        private ContactPoint(string? name, string? telephone, string? email, string? faxNumber, string? url)
        {
            Name = name;
            Telephone = telephone;
            Email = email;
            FaxNumber = faxNumber;
            Url = url;
        }

        public static ContactPoint Create(string? name, string? telephone, string? email, string? faxNumber = null, string? url = null)
            => new(name, telephone, email, faxNumber, url);

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Name;
            yield return Telephone;
            yield return Email;
            yield return FaxNumber;
            yield return Url;
        }
    }
}
