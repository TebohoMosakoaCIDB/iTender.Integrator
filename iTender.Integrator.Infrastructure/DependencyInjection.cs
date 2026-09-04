using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Infrastructure.Integrations.Ocds;
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

            return services;
        }
    }
}
