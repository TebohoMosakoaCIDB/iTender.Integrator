using System.Net;
using System.Text.Json;

namespace iTender.Integrator.Demo.Services
{
    public sealed class IntegratorApiClient(HttpClient httpClient)
    {
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<JsonElement> GetAsync(
            string relativeUrl,
            CancellationToken cancellationToken = default)
        {
            using var response = await httpClient.GetAsync(
                relativeUrl,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(
                    cancellationToken);

                throw new IntegratorApiException(
                    response.StatusCode,
                    $"API request failed: {(int)response.StatusCode} " +
                    $"{response.ReasonPhrase}. {body}");
            }

            await using var stream =
                await response.Content.ReadAsStreamAsync(cancellationToken);

            using var document =
                await JsonDocument.ParseAsync(
                    stream,
                    cancellationToken: cancellationToken);

            return document.RootElement.Clone();
        }

        public Task<JsonElement> GetOcdsReleasesAsync(
            int pageNumber = 1,
            int pageSize = 20,
            DateTime? from = null,
            DateTime? to = null,
            CancellationToken cancellationToken = default)
        {
            var query =
                $"api/Ocds/releases?pageNumber={pageNumber}&pageSize={pageSize}";

            if (from.HasValue)
            {
                query += $"&from={Uri.EscapeDataString(from.Value.ToString("O"))}";
            }

            if (to.HasValue)
            {
                query += $"&to={Uri.EscapeDataString(to.Value.ToString("O"))}";
            }

            return GetAsync(query, cancellationToken);
        }

        public Task<JsonElement> GetComplianceCheckedReleasesAsync(
            int pageNumber = 1,
            int pageSize = 20,
            DateTime? from = null,
            DateTime? to = null,
            CancellationToken cancellationToken = default)
        {
            var query =
                $"api/Ocds/releases/compliance-checked" +
                $"?pageNumber={pageNumber}&pageSize={pageSize}";

            if (from.HasValue)
            {
                query += $"&from={Uri.EscapeDataString(from.Value.ToString("O"))}";
            }

            if (to.HasValue)
            {
                query += $"&to={Uri.EscapeDataString(to.Value.ToString("O"))}";
            }

            return GetAsync(query, cancellationToken);
        }

        public Task<JsonElement> GetContractorAsync(
            string crsNumber,
            CancellationToken cancellationToken = default)
        {
            return GetAsync(
                $"api/Contractors/{Uri.EscapeDataString(crsNumber)}",
                cancellationToken);
        }

        public Task<JsonElement> GetCsdSupplierAsync(
            string maaANumber,
            CancellationToken cancellationToken = default)
        {
            return GetAsync(
                $"api/Csd/suppliers/{Uri.EscapeDataString(maaANumber)}",
                cancellationToken);
        }

        public Task<JsonElement> GetTrackRecordAsync(
             string crsNumber,             
            CancellationToken cancellationToken = default)
        {
            return GetAsync(
                $"api/TrackRecords/{Uri.EscapeDataString(crsNumber)}",
                cancellationToken);
        }

        public Task<JsonElement> GetComplianceSupplierAsync(
            string maaANumber,
            string registrationScheme = "ZA-CSD",
            CancellationToken cancellationToken = default)
        {
            return GetAsync(
                $"api/Compliance/suppliers/" +
                $"{Uri.EscapeDataString(maaANumber)}" +
                $"?registrationScheme={Uri.EscapeDataString(registrationScheme)}",
                cancellationToken);
        }
    }

    public sealed class IntegratorApiException(
        HttpStatusCode statusCode,
        string message)
        : Exception(message)
    {
        public HttpStatusCode StatusCode { get; } = statusCode;
    }
}
