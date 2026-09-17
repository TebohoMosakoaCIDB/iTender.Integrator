namespace iTender.Integrator.Application.Interfaces
{
    public interface ICsdAuthTokenProvider
    {
        Task<Guid> GetTokenAsync(CancellationToken cancellationToken = default);
    }
}
