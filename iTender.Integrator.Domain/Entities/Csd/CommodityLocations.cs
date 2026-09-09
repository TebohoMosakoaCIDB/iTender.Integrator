using System.Xml.Serialization;

namespace iTender.Integrator.Domain.Entities.Csd
{
    public class CommodityLocations
    {
        [XmlElement("WardCode")]
        public List<string> WardCodes { get; set; } = [];
    }
}
