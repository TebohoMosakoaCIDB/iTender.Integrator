namespace iTender.Integrator.Application.DTOs.Ocds
{
    public class OcdsTechniquesDto
    {
        public bool HasFrameworkAgreement { get; set; }

        public OcdsFrameworkAgreementDto? FrameworkAgreement { get; set; }

        public bool HasElectronicAuction { get; set; }

        public OcdsElectronicAuctionDto? ElectronicAuction { get; set; }
    }
}
