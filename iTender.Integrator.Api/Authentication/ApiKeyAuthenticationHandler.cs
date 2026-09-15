using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace iTender.Integrator.Api.Authentication
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationSchemeOptions>
    {
        private readonly IOptionsMonitor<ApiKeyOptions> _apiKeyOptions;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IOptionsMonitor<ApiKeyOptions> apiKeyOptions)
            : base(options, logger, encoder)
        {
            _apiKeyOptions = apiKeyOptions;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyDefaults.HeaderName, out var provided)
                || string.IsNullOrWhiteSpace(provided))
            {
                return Task.FromResult(AuthenticateResult.Fail($"Missing {ApiKeyDefaults.HeaderName} header."));
            }

            var configuredKeys = _apiKeyOptions.CurrentValue.Keys;

            if (configuredKeys.Count == 0)
            {
                // Fails closed, not open - a misconfigured (empty) key list rejects
                // every request rather than silently allowing everything through.
                Logger.LogError(
                    "No API keys configured under {Section} - every request will be rejected until at least one key is added.",
                    ApiKeyOptions.SectionName);
                return Task.FromResult(AuthenticateResult.Fail("No API keys are configured."));
            }

            var providedKey = provided.ToString();
            var match = configuredKeys.FirstOrDefault(k => TimingSafeEquals(k.Key, providedKey));

            if (match is null)
            {
                Logger.LogWarning("Rejected request with an unrecognised API key.");
                return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, match.Label),
                new Claim("api_key_label", match.Label)
            };
            var identity = new ClaimsIdentity(claims, ApiKeyDefaults.SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, ApiKeyDefaults.SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        // Compares SHA-256 digests of both strings via a fixed-time comparison,
        // rather than the raw strings directly - CryptographicOperations.FixedTimeEquals
        // requires equal-length inputs, and hashing first sidesteps the length check
        // itself becoming a timing signal (a raw length-mismatch short-circuit would
        // leak how many characters of the key were even worth comparing).
        private static bool TimingSafeEquals(string configuredKey, string providedKey)
        {
            var configuredHash = SHA256.HashData(Encoding.UTF8.GetBytes(configuredKey));
            var providedHash = SHA256.HashData(Encoding.UTF8.GetBytes(providedKey));
            return CryptographicOperations.FixedTimeEquals(configuredHash, providedHash);
        }
    }
}
