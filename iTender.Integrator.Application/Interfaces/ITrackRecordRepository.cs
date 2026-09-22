using iTender.Integrator.Application.DTOs.Crm;

namespace iTender.Integrator.Application.Interfaces
{
    public interface ITrackRecordRepository
    {
        IEnumerable<TrackRecordModel> GetByCrsNumberAsync(string contractorCsd);
    }
}
