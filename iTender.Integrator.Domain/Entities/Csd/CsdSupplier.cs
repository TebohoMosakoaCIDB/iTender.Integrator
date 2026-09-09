
using System.Xml.Serialization;

namespace iTender.Integrator.Domain.Entities.Csd
{
    [XmlRoot("Supplier")]
    public class CsdSupplier
    {
        [XmlElement("SupplierIdentification")]
        public SupplierIdentification SupplierIdentification { get; set; } = new();

        [XmlArray("IndustryClassifications")]
        [XmlArrayItem("IndustryClassification")]
        public List<IndustryClassification> IndustryClassifications { get; set; } = [];

        [XmlArray("Contacts")]
        [XmlArrayItem("Contact")]
        public List<Contact> Contacts { get; set; } = [];

        [XmlArray("Addresses")]
        [XmlArrayItem("Address")]
        public List<Address> Addresses { get; set; } = [];

        [XmlArray("BankAccounts")]
        [XmlArrayItem("BankAccount")]
        public List<BankAccount> BankAccounts { get; set; } = [];

        [XmlElement("Tax")]
        public Tax Tax { get; set; } = new();

        [XmlArray("Directors")]
        [XmlArrayItem("Director")]
        public List<Director> Directors { get; set; } = [];

        [XmlArray("OwnershipNonNaturals")]
        [XmlArrayItem("OwnershipNonNatural")]
        public List<OwnershipNonNatural> OwnershipNonNaturals { get; set; } = [];

        [XmlArray("Associations")]
        [XmlArrayItem("Association")]
        public List<Association> Associations { get; set; } = [];

        [XmlArray("CommodityGroups")]
        [XmlArrayItem("CommodityGroup")]
        public List<CommodityGroup> CommodityGroups { get; set; } = [];

        [XmlArray("Accreditations")]
        [XmlArrayItem("Accreditation")]
        public List<Accreditation> Accreditations { get; set; } = [];

        [XmlElement("BBBEE")]
        public Bbee? BBBEE { get; set; }
    }
}
