using System.Xml.Serialization;

namespace iTender.Integrator.Domain.Entities.Csd
{
    public class Accreditation
    {
        [XmlElement("AccreditationID")]
        public int AccreditationId { get; set; }

        [XmlElement("AccreditationNumber")]
        public string? AccreditationNumber { get; set; }

        [XmlElement("Description")]
        public string? Description { get; set; }

        [XmlElement("RegistrationDate")]
        public DateTime? RegistrationDate { get; set; }

        [XmlElement("ExpiryDate")]
        public DateTime? ExpiryDate { get; set; }

        [XmlElement("IsVerified")]
        public bool IsVerified { get; set; }

        [XmlElement("LastVerificationDate")]
        public DateTime? LastVerificationDate { get; set; }

        [XmlElement("IsActive")]
        public bool IsActive { get; set; }

        [XmlElement("StatusCode")]
        public string? StatusCode { get; set; }

        [XmlElement("AccreditationBody")]
        public AccreditationBody? AccreditationBody { get; set; }

        [XmlElement("CreatedDate")]
        public DateTime? CreatedDate { get; set; }

        [XmlElement("EditDate")]
        public DateTime? EditDate { get; set; }

        public bool IsExpired(DateTime asOfUtc) => ExpiryDate is not null && ExpiryDate.Value <= asOfUtc;
    }

    public class AccreditationBody
    {
        // Business code from CSD's master data list. The literal value that identifies
        // "cidb" as the accrediting body is NOT documented in the job aid we have (it
        // just says "refer to master data list") - confirm the real code against a live
        // CSD response or the master data list before relying on
        // CsdApiOptions.CidbAccreditationBodyCode below.
        [XmlElement("AccreditationBodyCode")]
        public string? AccreditationBodyCode { get; set; }
    }
}
