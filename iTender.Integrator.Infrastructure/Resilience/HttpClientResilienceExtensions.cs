using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace iTender.Integrator.Infrastructure.Resilience
{
    public static class HttpClientResilienceExtensions
    {
        /// <summary>
        /// Applies retry-with-backoff to an HttpClient registered via AddHttpClient,
        /// for CSD and National Treasury's OCDS feed - both plain HTTP APIs with no
        /// resilience of their own. Deliberately NOT applied to CRM: Dataverse's
        /// ServiceClient already retries transient failures and 429 throttling
        /// internally (see CrmServiceFactory) - wrapping an already-retrying client
        /// in another retry layer would just compound delays on a real outage
        /// rather than add any real protection.
        ///
        /// IMPORTANT: the caller must set client.Timeout to Timeout.InfiniteTimeSpan
        /// (not a finite value) when registering the client this is attached to.
        /// HttpClient.Timeout wraps the ENTIRE handler pipeline as one call,
        /// retries included - a finite value there would cut off the whole retry
        /// sequence after roughly one attempt, not just one network call. The
        /// AddTimeout calls below are the real timeout enforcement instead.
        ///
        /// timeoutSeconds is the budget for ONE attempt (matching what
        /// CsdApiOptions.TimeoutSeconds/OcdsApiOptions.TimeoutSeconds already meant
        /// before this existed).
        /// </summary>
        public static IHttpClientBuilder AddDefaultResilience(
            this IHttpClientBuilder builder, int timeoutSeconds)
        {
            var attemptTimeout = TimeSpan.FromSeconds(Math.Max(1, timeoutSeconds));

            // Retry x3 + the original attempt = up to 4 attempts; generous ceiling
            // so this caps the whole sequence without becoming the binding
            // constraint ahead of the retry policy itself.
            var totalTimeout = attemptTimeout * 5;

            builder.AddResilienceHandler(
                $"{builder.Name}-resilience",
                pipelineBuilder =>
                {
                    pipelineBuilder.AddTimeout(totalTimeout);

                    pipelineBuilder.AddRetry(new HttpRetryStrategyOptions
                    {
                        MaxRetryAttempts = 3,
                        BackoffType = DelayBackoffType.Exponential,
                        UseJitter = true,
                        Delay = TimeSpan.FromSeconds(1)
                    });

                    pipelineBuilder.AddTimeout(attemptTimeout);
                });

            return builder;
        }
    }
}
