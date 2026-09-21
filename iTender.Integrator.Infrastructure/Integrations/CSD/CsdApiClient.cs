using iTender.Integrator.Application.DTOs.Csd;
using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Domain.Entities.Csd;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;

namespace iTender.Integrator.Infrastructure.Integrations.CSD
{
    public class CsdApiClient : ICsdApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ICsdAuthTokenProvider _tokenProvider;
        private readonly CsdApiOptions _options;

        public CsdApiClient(
            HttpClient httpClient,
            ICsdAuthTokenProvider tokenProvider,
            IOptions<CsdApiOptions> options)
        {
            _httpClient = httpClient;
            _tokenProvider = tokenProvider;
            _options = options.Value;
        }

        public async Task<CsdSupplier> GetSupplierDetailsAsync(
            string supplierNumber,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(supplierNumber))
            {
                throw new ArgumentException(
                    "Supplier number is required.",
                    nameof(supplierNumber));
            }

            // No more EnsureAuthenticatedAsync/_authentication field here - this
            // typed client is transient (AddHttpClient<TClient,TImpl> registers it
            // that way), so a token cached on an instance field never actually
            // persisted between calls. ICsdAuthTokenProvider is a genuine
            // singleton and does the real caching now.
            var token = await _tokenProvider.GetTokenAsync(cancellationToken);

            var request = new GetSupplierDetailRequest
            {
                SupplierNumber = supplierNumber
            };

            var xml = CsdXmlSerializer.Serialize(request);

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                _options.SupplierDetailsEndpoint);

            httpRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token.ToString());

            httpRequest.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/xml"));

            httpRequest.Headers.Add("csdversion", "2");

            httpRequest.Content = new StringContent(
                xml,
                Encoding.UTF8,
                "application/xml");

            using var response = await _httpClient.SendAsync(
                    httpRequest,
                    cancellationToken);

            var responseXml = await response.Content
                .ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"CSD supplier request failed. " +
                    $"Status: {(int)response.StatusCode} {response.StatusCode}. " +
                    $"Response: {responseXml}");
            }

            return CsdXmlSerializer.Deserialize<CsdSupplier>(responseXml);
        }
    }
}
