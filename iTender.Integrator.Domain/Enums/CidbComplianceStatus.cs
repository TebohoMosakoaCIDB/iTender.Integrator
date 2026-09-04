namespace iTender.Integrator.Domain.Enums
{
    public enum CidbComplianceStatus
    {
        NotChecked = 0,
        Compliant,
        NonCompliant,
        GradingExpired,
        GradingInsufficient,
        RegistrationNotFound,
        RegistrationSuspended
    }
}
