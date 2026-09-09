using System.Xml.Serialization;

namespace iTender.Integrator.Application.DTOs.Csd
{
    [XmlRoot("GetSupplierDetailRequest")]
    public class GetSupplierDetailRequest
    {
        [XmlElement("SupplierNumber")]
        public string SupplierNumber { get; set; } = string.Empty;
    }
}
