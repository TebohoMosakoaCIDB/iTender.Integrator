using iTender.Integrator.Domain.Entities.Csd;

namespace iTender.Integrator.Application.Interfaces
{
    public interface ICsdApiClient
    {
        Task<CsdSupplier> GetSupplierDetailsAsync(
            string supplierNumber,
            CancellationToken cancellationToken = default);
    }
}
