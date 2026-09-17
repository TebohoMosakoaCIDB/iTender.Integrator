using iTender.Integrator.Demo.Configuration;
using Microsoft.Extensions.Options;

namespace iTender.Integrator.Demo.Infrastructure
{
    public sealed class IntegratorApiKeyHandler(
    IOptions<IntegratorApiOptions> options)
    : DelegatingHandler
    {
        private readonly IntegratorApiOptions _options = options.Value;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                request.Headers.Remove("X-Api-Key");

                request.Headers.TryAddWithoutValidation(
                    "X-Api-Key",
                    _options.ApiKey);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
