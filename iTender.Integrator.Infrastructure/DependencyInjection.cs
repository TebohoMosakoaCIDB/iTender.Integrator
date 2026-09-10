using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Infrastructure.Integrations.CRM;
using iTender.Integrator.Infrastructure.Integrations.CSD;
using iTender.Integrator.Infrastructure.Integrations.Ocds;
using iTender.Integrator.Infrastructure.Repositories;
using iTender.Integrator.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace iTender.Integrator.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {

            //National treasury API
            services.Configure<OcdsApiOptions>(
                configuration.GetSection(OcdsApiOptions.SectionName));

            services.AddHttpClient<IOcdsApiClient, OcdsApiClient>(
                (serviceProvider, client) =>
                {
                    var options = serviceProvider
                        .GetRequiredService<IOptions<OcdsApiOptions>>()
                        .Value;

                    client.BaseAddress = new Uri(options.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
                });

            //Csd API
            services.Configure<CsdApiOptions>(
                configuration.GetSection(CsdApiOptions.SectionName));

            services.AddHttpClient<ICsdApiClient, CsdApiClient>(
                (serviceProvider, client) =>
                {
                    var options = serviceProvider
                        .GetRequiredService<IOptions<CsdApiOptions>>()
                        .Value;

                    client.BaseAddress = new Uri(options.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
                });

            //CRM connection
            services.AddOptions<CrmOptions>()
                .Bind(configuration.GetSection(CrmOptions.SectionName))
                .ValidateOnStart();

            services.AddOptions<EncryptionOptions>()
                .Bind(configuration.GetSection(EncryptionOptions.SectionName))
                .ValidateOnStart();

            services.AddScoped<ICrmServiceFactory, CrmServiceFactory>();

            services.AddScoped<IContractorGradeRepository, ContractorGradeRepository>();

            services.AddScoped<IContractorComplianceService, ContractorComplianceService>();

            services.AddScoped<IReleaseComplianceService, ReleaseComplianceService>();

            services.AddScoped<ITenderRepository, TenderRepository>();

            services.AddScoped<IProvinceRepository, ProvinceRepository>();

            services.AddScoped<IMetroDistrictRepository, MetroDistrictRepository>();

            services.AddScoped<IClassOfWorkTypeRepository, ClassOfWorkTypeRepository>();

            return services;
        }
    }
}
