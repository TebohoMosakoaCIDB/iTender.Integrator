using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Infrastructure.Integrations.CRM;
using iTender.Integrator.Infrastructure.Integrations.Ocds;
using iTender.Integrator.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iTender.Integrator.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<OcdsApiOptions>(
                configuration.GetSection(OcdsApiOptions.SectionName));

            services.AddHttpClient<IOcdsApiClient, OcdsApiClient>(
                (serviceProvider, client) =>
                {
                    var options = serviceProvider
                        .GetRequiredService<
                            Microsoft.Extensions.Options.IOptions<OcdsApiOptions>>()
                        .Value;

                    client.BaseAddress = new Uri(options.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(
                        options.TimeoutSeconds);
                });

            services.AddOptions<CrmOptions>()
                .Bind(configuration.GetSection(CrmOptions.SectionName))
                .ValidateOnStart();

            services.AddOptions<EncryptionOptions>()
                .Bind(configuration.GetSection(EncryptionOptions.SectionName))
                .ValidateOnStart();

            services.AddScoped<ICrmServiceFactory, CrmServiceFactory>();

            services.AddScoped<IContractorRepository, ContractorRepository>();
            //services.AddScoped<IContractorGradeRepository, ContractorGradeRepository>();

            return services;
        }
    }
}
