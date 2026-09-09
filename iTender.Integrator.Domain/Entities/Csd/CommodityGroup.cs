using System.Xml.Serialization;

namespace iTender.Integrator.Domain.Entities.Csd
{
    public class CommodityGroup
    {
        [XmlElement("CommodityGroupID")]
        public int CommodityGroupID { get; set; }

        [XmlElement("Name")]
        public string? Name { get; set; }

        [XmlElement("Description")]
        public string? Description { get; set; }

        [XmlElement("NationWide")]
        public bool NationWide { get; set; }

        [XmlElement("ProvinceWide")]
        public bool ProvinceWide { get; set; }

        [XmlElement("IsActive")]
        public bool IsActive { get; set; }

        [XmlElement("CommodityItems")]
        public CommodityItem? CommodityItems { get; set; }

        [XmlElement("CommodityLocations")]
        public CommodityLocations? CommodityLocations { get; set; }

        [XmlElement("CommodityProvinces")]
        public CommodityProvinces? CommodityProvinces { get; set; }

        [XmlElement("Field1")]
        public string? Field1 { get; set; }

        [XmlElement("Field2")]
        public string? Field2 { get; set; }

        [XmlElement("Field3")]
        public string? Field3 { get; set; }

        [XmlElement("CreatedDate")]
        public DateTime? CreatedDate { get; set; }

        [XmlElement("EditDate")]
        public DateTime? EditDate { get; set; }
    }
}
