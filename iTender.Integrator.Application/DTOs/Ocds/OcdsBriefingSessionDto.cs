namespace iTender.Integrator.Application.DTOs.Ocds
{
    public class OcdsBriefingSessionDto
    {
        public bool IsSession { get; set; }

        public bool Compulsory { get; set; }

        public string? Date { get; set; }

        public string? Venue { get; set; }
    }
}
