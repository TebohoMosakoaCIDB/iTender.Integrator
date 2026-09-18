using iTender.Integrator.Application.DTOs.eTenders;

namespace iTender.Integrator.Application.Interfaces
{
    public interface IETendersAdminApiClient
    {
        Task<EtendersApiResponse> CreateTenderAsync(
            CreateTenderRequest request,
            IReadOnlyCollection<TenderDocumentUpload>? documents = null,
            CancellationToken cancellationToken = default);
    }
}
