using System.Xml.Serialization;

namespace iTender.Integrator.Domain.Entities.Csd
{
    public class OwnershipDemographic
    {
        [XmlElement("OwnershipDemographicID")]
        public int OwnershipDemographicID { get; set; }

        [XmlElement("GenderCode")]
        public string? GenderCode { get; set; }

        [XmlElement("EthnicGroupCode")]
        public string? EthnicGroupCode { get; set; }

        [XmlElement("Percentage")]
        public decimal Percentage { get; set; }
    }
}
