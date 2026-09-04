using iTender.Integrator.Domain.Enums;
using iTender.Integrator.Domain.ValueObjects;

namespace iTender.Integrator.Domain.Events
{
    public sealed record TenderPublishedDomainEvent(
         string Ocid,
         Guid TenderId,
         string Title,
         DateTime OccurredOnUtc) : IDomainEvent;

    // Feeds project registration on iTender once an award goes Active.
    public sealed record TenderAwardedDomainEvent(
        string Ocid,
        Guid AwardId,
        Money? Value,
        DateTime OccurredOnUtc) : IDomainEvent;

    // Point #3 - real-time contractor/project matching.
    public sealed record ContractorMatchedDomainEvent(
        Guid ContractorId,
        string Ocid,
        string TenderExternalId,
        DateTime OccurredOnUtc) : IDomainEvent;

    // Point #2/#5 - compliance enforcement and anti-fraud monitoring.
    public sealed record ComplianceViolationDetectedDomainEvent(
        Guid ContractorId,
        string CidbRegistrationNumber,
        CidbComplianceStatus Status,
        DateTime OccurredOnUtc) : IDomainEvent;

    // Point #5 - flags when a re-synced release disagrees with what we stored previously.
    public sealed record TenderDataInconsistencyDetectedDomainEvent(
        string Ocid,
        string FieldName,
        string? PreviousValue,
        string? NewValue,
        DateTime OccurredOnUtc) : IDomainEvent;
}
