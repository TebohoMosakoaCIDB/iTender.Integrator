namespace iTender.Integrator.Domain.Events
{
    public interface IDomainEvent
    {
        DateTime OccurredOnUtc { get; }
    }
}
