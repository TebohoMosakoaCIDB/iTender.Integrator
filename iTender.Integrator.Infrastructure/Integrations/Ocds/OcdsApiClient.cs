using iTender.Integrator.Application.DTOs.Ocds;
using iTender.Integrator.Application.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace iTender.Integrator.Infrastructure.Integrations.Ocds
{
    public class OcdsApiClient : IOcdsApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly OcdsApiOptions _options;

        public OcdsApiClient(
            HttpClient httpClient,
            IOptions<OcdsApiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<OcdsReleasePackageDto> GetReleasesAsync(
            int PageNumber,
            int PageSize,
            DateTime? from,
            DateTime? to,
            CancellationToken cancellationToken = default)
        {
            var endpoint = _options.ReleasesEndpoint;

            var query = new List<string>();

            if (PageNumber != 0)
            {
                query.Add($"PageNumber={Uri.EscapeDataString(PageNumber.ToString())}");
            }

            if (PageSize != 0)
            {
                query.Add($"PageSize={Uri.EscapeDataString(PageSize.ToString())}");
            }

            if (from.HasValue)
            {
                query.Add($"dateFrom={Uri.EscapeDataString(from.Value.ToString("yyyy-MM-dd"))}");
            }

            if (to.HasValue)
            {
                query.Add($"dateTo={Uri.EscapeDataString(to.Value.ToString("yyyy-MM-dd"))}");
            }

            if (query.Count > 0)
            {
                endpoint += endpoint.Contains('?')
                    ? "&" + string.Join("&", query)
                    : "?" + string.Join("&", query);
            }

            var response = await _httpClient.GetAsync(
                endpoint,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadFromJsonAsync<OcdsReleasePackageDto>(
                    cancellationToken);

            return result ?? new OcdsReleasePackageDto();
        }
    }
}
