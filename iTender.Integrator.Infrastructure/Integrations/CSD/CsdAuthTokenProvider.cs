using iTender.Integrator.Application.DTOs.Csd;
using iTender.Integrator.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;

namespace iTender.Integrator.Infrastructure.Integrations.CSD
{
    public sealed class CsdAuthTokenProvider : ICsdAuthTokenProvider, IDisposable
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CsdApiOptions _options;
        private readonly ILogger<CsdAuthTokenProvider> _logger;
        private readonly SemaphoreSlim _lock = new(1, 1);

        private AuthenticationResponse? _cached;

        public CsdAuthTokenProvider(
            IHttpClientFactory httpClientFactory,
            IOptions<CsdApiOptions> options,
            ILogger<CsdAuthTokenProvider> logger)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<Guid> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            if (IsStillValid(_cached))
                return _cached!.Token;

            await _lock.WaitAsync(cancellationToken);
            try
            {
                // Re-check after acquiring the lock - someone else may have already
                // refreshed it while this call was waiting.
                if (IsStillValid(_cached))
                    return _cached!.Token;

                _cached = await AuthenticateAsync(cancellationToken);
                return _cached.Token;
            }
            finally
            {
                _lock.Release();
            }
        }

        // NOTE: TokenExpireDateTime comes from CSD's response with an unconfirmed
        // timezone convention, compared here against DateTime.Now (local, not UTC)
        // - unchanged from the original code rather than silently "fixed" without
        // evidence of which timezone CSD actually returns. If tokens ever expire
        // earlier or later than expected, this comparison is the first place to
        // check, not just the CSD account settings.
        private static bool IsStillValid(AuthenticationResponse? token)
            => token is not null && token.TokenExpireDateTime > DateTime.Now.AddMinutes(1);

        private async Task<AuthenticationResponse> AuthenticateAsync(CancellationToken cancellationToken)
        {
            var request = new AuthenticationRequest
            {
                AcceptTermsandConditions = true,
                Email = _options.Username,
                Password = _options.Password
            };

            var xml = CsdXmlSerializer.Serialize(request);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, _options.AuthenticateEndpoint);

            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            httpRequest.Content = new StringContent(xml, Encoding.UTF8, "application/xml");

            // Named client, not the ICsdApiClient typed client - this deliberately
            // doesn't depend on ICsdApiClient/CsdApiClient at all, so there's no
            // circular dependency between "the client that needs a token" and "the
            // thing that provides tokens".
            var httpClient = _httpClientFactory.CreateClient(CsdAuthHttpClientName);

            using var response = await httpClient.SendAsync(httpRequest, cancellationToken);
            var responseXml = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"CSD authentication failed. Status: {(int)response.StatusCode} {response.StatusCode}. " +
                    $"Response: {responseXml}");
            }

            var authentication = CsdXmlSerializer.Deserialize<AuthenticationResponse>(responseXml);

            ValidateAuthentication(authentication);

            return authentication;
        }

        // Re-enabled, not new: this existed in the original CsdApiClient but was
        // commented out (//ValidateAuthentication();) - meaning a suspended, locked,
        // or unverified CSD account was previously being used as if it were valid.
        // That looked like an oversight rather than a deliberate choice, so it's
        // restored here rather than left disabled.
        private static void ValidateAuthentication(AuthenticationResponse authentication)
        {
            if (authentication.IsSuspended)
                throw new InvalidOperationException("CSD account is suspended.");

            if (authentication.LockoutEnabled)
                throw new InvalidOperationException("CSD account is locked.");

            if (!authentication.IsAccountActive)
                throw new InvalidOperationException("CSD account is inactive.");

            if (!authentication.IsAccountVerified)
                throw new InvalidOperationException("CSD account has not been verified.");

            if (authentication.IsPasswordExpired)
                throw new InvalidOperationException("CSD account password has expired.");

            if (authentication.Token == Guid.Empty)
                throw new InvalidOperationException("CSD authentication returned an invalid token.");
        }

        public const string CsdAuthHttpClientName = "CsdAuth";

        public void Dispose() => _lock.Dispose();
    }
}
