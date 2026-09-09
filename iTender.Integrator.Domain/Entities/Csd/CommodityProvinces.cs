using System.Xml.Serialization;

namespace iTender.Integrator.Domain.Entities.Csd
{
    public class CommodityProvinces
    {
        [XmlElement("ProvinceCode")]
        public List<string> ProvinceCodes { get; set; } = [];
    }
}
