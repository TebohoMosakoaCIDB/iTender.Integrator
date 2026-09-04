namespace iTender.Integrator.Application.DTOs.Ocds
{
    public class OcdsOtherRequirementsDto
    {
        public List<string> ReservedParticipation { get; set; } = [];

        public bool RequiresStaffNamesAndQualifications { get; set; }
    }
}
