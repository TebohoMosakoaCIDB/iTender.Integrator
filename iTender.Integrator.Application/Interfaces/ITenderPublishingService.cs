using iTender.Integrator.Application.DTOs.eTenders;
using iTender.Integrator.Application.DTOs.ETenders;

namespace iTender.Integrator.Application.Interfaces
{
    public interface ITenderPublishingService
    {
        Task<CreateTenderResult> CreateTenderAsync(
            CreateTenderRequest request,
            IReadOnlyCollection<TenderDocumentUpload>? documents = null,
            CancellationToken cancellationToken = default);
    }
}
