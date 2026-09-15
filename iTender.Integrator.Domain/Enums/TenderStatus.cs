namespace iTender.Integrator.Domain.Enums
{
    public enum TenderStatus
    {
        Unknown = 0,
        DRAFT_STATUS = 1,
        ADVERTISED_STATUS = 100000000,
        CANCELLED_STATUS = 100000001,
        CLOSED_STATUS = 100000002
    }
}
