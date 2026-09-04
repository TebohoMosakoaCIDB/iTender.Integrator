namespace iTender.Integrator.Application.DTOs.Ocds
{
    public class OcdsPeriodDto
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? MaxExtentDate { get; set; }

        public int? DurationInDays { get; set; }
    }
}
