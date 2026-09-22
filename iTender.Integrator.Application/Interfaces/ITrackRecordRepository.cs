using iTender.Integrator.Application.DTOs.Crm;

namespace iTender.Integrator.Application.Interfaces
{
    public interface ITrackRecordRepository
    {
        Task<IReadOnlyCollection<TrackRecordModel>> GetByCrsNumberAsync(
            string crsNumber, CancellationToken cancellationToken = default);
    }
}
