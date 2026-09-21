namespace iTender.Integrator.Application.DTOs.Compliance
{
    public sealed record ContractorNotificationOutcome(
        string ContractorName,
        string? CrsNumber,
        bool Sent,
        string Channel,
        string? Reason);

    public sealed record NotifyQualifiedContractorsResult(
        int QualifiedCount,
        int SentCount,
        IReadOnlyCollection<ContractorNotificationOutcome> Outcomes);
}