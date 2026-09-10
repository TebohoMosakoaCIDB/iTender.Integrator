using iTender.Integrator.Application.DTOs.Crm;

namespace iTender.Integrator.Application.Interfaces
{
    /// <summary>
    /// Publishes a tender into CRM (nv_tender) - the "advertisement" step from the
    /// board deck's integration layer (slide 4), and the "Publish and monitor" step
    /// of slide 7's CLIENT -> eTENDER -> API GATEWAY -> iTENDER pipeline.
    /// </summary>
    public interface ITenderRepository
    {
        /// <summary>
        /// Creates the tender if none exists with this EmployerTenderNumber, or
        /// updates the existing one. Returns the CRM record id either way.
        /// </summary>
        Task<Guid> UpsertAsync(CreateTenderModel model, CancellationToken cancellationToken = default);
    }
}
