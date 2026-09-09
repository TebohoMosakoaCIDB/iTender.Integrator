using System.Xml.Serialization;

namespace iTender.Integrator.Domain.Entities.Csd
{
    public class CommodityItem
    {
        [XmlElement("CommodityCode")]
        public string? CommodityCode { get; set; }
    }
}
