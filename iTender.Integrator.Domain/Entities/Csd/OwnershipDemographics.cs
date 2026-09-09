using System.Xml.Serialization;

namespace iTender.Integrator.Domain.Entities.Csd
{
    public class OwnershipDemographics
    {
        [XmlElement("OwnershipDemographic")]
        public List<OwnershipDemographic> Items { get; set; } = [];
    }
}
