using Microsoft.Xrm.Sdk;

namespace iTender.Integrator.Infrastructure.Integrations.CRM
{
    public interface ICrmServiceFactory
    {
        IOrganizationService Create();
    }
}
