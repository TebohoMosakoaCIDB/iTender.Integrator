using System.Xml.Serialization;

namespace iTender.Integrator.Application.DTOs.Csd
{
    [XmlRoot("AuthenticationRequest")]
    public class AuthenticationRequest
    {
        [XmlElement("AcceptTermsandConditions")] 
        public bool AcceptTermsandConditions { get; set; }

        [XmlElement("Email")]
        public string Email { get; set; } = string.Empty;

        [XmlElement("Password")]
        public string Password { get; set; } = string.Empty;
    }
}
