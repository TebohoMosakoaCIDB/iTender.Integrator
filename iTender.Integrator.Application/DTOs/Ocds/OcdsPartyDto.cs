namespace iTender.Integrator.Application.DTOs.Ocds
{
    public class OcdsPartyDto
    {
        public string? Name { get; set; }

        public string? Id { get; set; }

        public OcdsIdentifierDto? Identifier { get; set; }

        public OcdsAddressDto? Address { get; set; }

        public OcdsContactPointDto? ContactPoint { get; set; }

        public List<string> Roles { get; set; } = [];

        public OcdsPartyDetailsDto? Details { get; set; }
    }
}
