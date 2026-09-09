using System.Xml.Serialization;

namespace iTender.Integrator.Domain.Entities.Csd
{
    public class Director
    {
        public int DirectorID { get; set; }

        public string? Name { get; set; }

        public string? Surname { get; set; }

        public DateTime? AppointmentDate { get; set; }

        public bool IsActive { get; set; }

        public string? CountryTypeCode { get; set; }

        public string? IDTypeCode { get; set; }

        public string? DirectorStatusTypeCode { get; set; }

        public string? SAIDNumber { get; set; }

        public string? ForeignIDNumber { get; set; }

        public string? ForeignPassportNumber { get; set; }

        public string? WorkPermitNumber { get; set; }

        public DateTime? LastVerificationDate { get; set; }

        [XmlArray("DirectorTypes")]
        [XmlArrayItem("DirectorType")]
        public List<DirectorType> DirectorTypes { get; set; } = [];

        [XmlElement("IsOwner")]
        public bool IsOwner { get; set; }

        [XmlElement("CellphoneNumber")]
        public string? CellphoneNumber { get; set; }

        [XmlElement("EmailAddress")]
        public string? EmailAddress { get; set; }

        [XmlElement("GenderCode")]
        public string? GenderCode { get; set; }

        [XmlElement("EthnicGroupCode")]
        public string? EthnicGroupCode { get; set; }

        [XmlElement("OwnershipPercentage")]
        public decimal OwnershipPercentage { get; set; }

        [XmlElement("OwnershipDemographics")]
        public OwnershipDemographics? OwnershipDemographics { get; set; }

        [XmlArray("DirectorFlags")]
        [XmlArrayItem("DirectorFlag")]
        public List<DirectorFlag> DirectorFlags { get; set; } = [];

        public string? Field1 { get; set; }

        public string? Field2 { get; set; }

        public string? Field3 { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? EditDate { get; set; }

        public int DemographicsID { get; set; }

        public int GenderID { get; set; }

        public int EthnicGroupID { get; set; }
    }
}
