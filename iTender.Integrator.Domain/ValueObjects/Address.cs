using iTender.Integrator.Domain.Common;

namespace iTender.Integrator.Domain.ValueObjects
{
    public sealed class Address : ValueObject
    {
        public string? StreetAddress { get; }

        public string? Locality { get; }

        public string? Region { get; }

        public string? PostalCode { get; }

        public string? CountryName { get; }

        private Address(string? streetAddress, string? locality, string? region, string? postalCode, string? countryName)
        {
            StreetAddress = streetAddress;
            Locality = locality;
            Region = region;
            PostalCode = postalCode;
            CountryName = countryName;
        }

        public static Address Create(string? streetAddress, string? locality, string? region, string? postalCode, string? countryName)
            => new(streetAddress, locality, region, postalCode, countryName);

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return StreetAddress;
            yield return Locality;
            yield return Region;
            yield return PostalCode;
            yield return CountryName;
        }
    }
}
