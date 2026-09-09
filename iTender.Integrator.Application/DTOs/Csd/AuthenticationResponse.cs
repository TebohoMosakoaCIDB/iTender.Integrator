using System.Xml.Serialization;

namespace iTender.Integrator.Application.DTOs.Csd
{
    [XmlRoot("AuthenticationResponse")]
    public class AuthenticationResponse
    {
        [XmlElement("Token")]
        public Guid Token { get; set; }

        [XmlElement("TokenCreatedDateTime")]
        public DateTime TokenCreatedDateTime { get; set; }

        [XmlElement("TokenExpireDateTime")]
        public DateTime TokenExpireDateTime { get; set; }

        [XmlElement("LockoutEnabled")]
        public bool LockoutEnabled { get; set; }

        [XmlElement("IsSuspended")]
        public bool IsSuspended { get; set; }

        [XmlElement("IsAccountActive")]
        public bool IsAccountActive { get; set; }

        [XmlElement("IsAccountVerified")]
        public bool IsAccountVerified { get; set; }

        [XmlElement("IsPasswordExpired")]
        public bool IsPasswordExpired { get; set; }

        [XmlElement("SuspendedUntil")]
        public DateTime? SuspendedUntil { get; set; }

        [XmlElement("IsTermsAndConditionsAccepted")]
        public bool IsTermsAndConditionsAccepted { get; set; }
    }
}
