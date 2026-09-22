using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Infrastructure.BackgroundJobs;
using iTender.Integrator.Infrastructure.Integrations.CRM;
using iTender.Integrator.Infrastructure.Integrations.CSD;
using iTender.Integrator.Infrastructure.Integrations.eTenders;
using iTender.Integrator.Infrastructure.Integrations.Ocds;
using iTender.Integrator.Infrastructure.Persistence;
using iTender.Integrator.Infrastructure.Repositories;
using iTender.Integrator.Infrastructure.Resilience;
using iTender.Integrator.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
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

            // Read synchronously here (not via IOptions<T>) specifically because
            // AddDefaultResilience runs now, at startup - the client-configure
            // lambda below that resolves IOptions<OcdsApiOptions> only runs later,
            // the first time something actually requests IOcdsApiClient.
            var ocdsTimeoutSeconds = configuration.GetValue(
                $"{OcdsApiOptions.SectionName}:TimeoutSeconds", 60);

            services.AddHttpClient<IOcdsApiClient, OcdsApiClient>(
                (serviceProvider, client) =>
                {
                    var options = serviceProvider
                        .GetRequiredService<IOptions<OcdsApiOptions>>()
                        .Value;

                    // Trailing slash required: HttpClient.BaseAddress drops the last
                    // path segment when combining with a relative URI otherwise -
                    // the exact bug that caused the original CSD 404, fixed here too
                    // now rather than assumed away.
                    client.BaseAddress = new Uri(
                        options.BaseUrl.EndsWith('/') ? options.BaseUrl : options.BaseUrl + "/");

                    // Infinite, not options.TimeoutSeconds - see
                    // HttpClientResilienceExtensions.AddDefaultResilience for why:
                    // HttpClient.Timeout wraps the whole retry pipeline, so a finite
                    // value here would kill the retries this is meant to add.
                    client.Timeout = Timeout.InfiniteTimeSpan;
                })
                .AddDefaultResilience(ocdsTimeoutSeconds);

            //Csd API
            services.Configure<CsdApiOptions>(
                configuration.GetSection(CsdApiOptions.SectionName));

            var csdTimeoutSeconds = configuration.GetValue(
                $"{CsdApiOptions.SectionName}:TimeoutSeconds", 60);

            services.AddHttpClient<ICsdApiClient, CsdApiClient>(
                (serviceProvider, client) =>
                {
                    var options = serviceProvider
                        .GetRequiredService<IOptions<CsdApiOptions>>()
                        .Value;

                    client.BaseAddress = new Uri(
                        options.BaseUrl.EndsWith('/') ? options.BaseUrl : options.BaseUrl + "/");

                    client.Timeout = Timeout.InfiniteTimeSpan;
                })
                .AddDefaultResilience(csdTimeoutSeconds);

            // Named client for CsdAuthTokenProvider - deliberately separate from
            // the ICsdApiClient typed client above so there's no dependency in
            // either direction between "the client that needs a token" and "the
            // thing that provides tokens". Same base address/resilience as the
            // main CSD client since it's hitting the same host.
            services.AddHttpClient(CsdAuthTokenProvider.CsdAuthHttpClientName, (serviceProvider, client) =>
            {
                var options = serviceProvider
                    .GetRequiredService<IOptions<CsdApiOptions>>()
                    .Value;

                client.BaseAddress = new Uri(
                    options.BaseUrl.EndsWith('/') ? options.BaseUrl : options.BaseUrl + "/");

                client.Timeout = Timeout.InfiniteTimeSpan;
            })
                .AddDefaultResilience(csdTimeoutSeconds);

            //CRM connection
            services.AddOptions<CrmOptions>()
                .Bind(configuration.GetSection(CrmOptions.SectionName))
                .ValidateOnStart();

            services.AddOptions<EncryptionOptions>()
                .Bind(configuration.GetSection(EncryptionOptions.SectionName))
                .ValidateOnStart();

            services.AddSingleton<ICsdAuthTokenProvider, CsdAuthTokenProvider>();

            services.AddScoped<ICrmServiceFactory, CrmServiceFactory>();

            services.AddScoped<IContractorRepository, ContractorRepository>();

            services.AddScoped<IContractorGradeRepository, ContractorGradeRepository>();

            services.AddScoped<IContractorComplianceService, ContractorComplianceService>();

            services.AddScoped<IContractorProfileService, ContractorProfileService>();

            services.AddScoped<IReleaseComplianceService, ReleaseComplianceService>();

            services.AddScoped<ITenderRepository, TenderRepository>();

            services.AddScoped<ITrackRecordRepository, TrackRecordRepository>();

            services.AddScoped<IProvinceRepository, ProvinceRepository>();

            services.AddScoped<IMetroDistrictRepository, MetroDistrictRepository>();

            services.AddScoped<IClassOfWorkTypeRepository, ClassOfWorkTypeRepository>();

            services.Configure<OcdsPullOptions>(configuration.GetSection(OcdsPullOptions.SectionName));
            services.AddHostedService<OcdsPullBackgroundService>();

            services.AddDbContext<ItenderIntegratorDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ItenderIntegrator")));

            services.AddScoped<IReleaseRepository, ReleaseRepository>();

            //eTenders Admin API (write side - creates tenders on admin-uat.etenders.gov.za)
            services.Configure<ETendersAdminApiOptions>(
                configuration.GetSection(ETendersAdminApiOptions.SectionName));

            var etendersTimeoutSeconds = configuration.GetValue(
                $"{ETendersAdminApiOptions.SectionName}:TimeoutSeconds", 60);

            services.AddHttpClient<IETendersAdminApiClient, ETendersAdminApiClient>(
                (serviceProvider, client) =>
                {
                    var options = serviceProvider
                        .GetRequiredService<IOptions<ETendersAdminApiOptions>>()
                        .Value;

                    client.BaseAddress = new Uri(
                        options.BaseUrl.EndsWith('/') ? options.BaseUrl : options.BaseUrl + "/");

                    client.Timeout = Timeout.InfiniteTimeSpan;
                })
                .AddDefaultResilience(etendersTimeoutSeconds);

            services.AddScoped<ITenderPublishingService, TenderPublishingService>();

            services.AddScoped<IQualifiedContractorFinder, QualifiedContractorFinder>();

            // No gateway configured yet - see LoggingNotificationSender. Swap this
            // registration for a real email/SMS sender when one exists; nothing
            // else here needs to change.
            services.AddScoped<INotificationSender, LoggingNotificationSender>();

            services.AddScoped<IContractorNotificationService, ContractorNotificationService>();

            return services;
        }
    }
}
