namespace iTender.Integrator.Domain.Entities.Csd
{
    public class Address
    {
        public int AddressID { get; set; }

        public bool IsPreferred { get; set; }

        public bool IsActive { get; set; }

        public string? AddressTypeCode { get; set; }

        public string? AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; }

        public string? CountryCode { get; set; }

        public string? ProvinceCode { get; set; }

        public string? DistrictCode { get; set; }

        public string? MunicipalityCode { get; set; }

        public string? CityCode { get; set; }

        public string? SuburbCode { get; set; }

        public string? WardCode { get; set; }

        public string? PostalCode { get; set; }

        public bool IsPostalAddress { get; set; }

        public bool IsDeliveryAddress { get; set; }

        public bool IsPhysicalAddress { get; set; }

        public bool IsPaymentAddress { get; set; }

        public string? Field1 { get; set; }

        public string? Field2 { get; set; }

        public string? Field3 { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? EditDate { get; set; }

        public string? AddressSource { get; set; }
    }
}
