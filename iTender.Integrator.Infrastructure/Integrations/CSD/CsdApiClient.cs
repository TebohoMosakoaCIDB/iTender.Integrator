using iTender.Integrator.Application.DTOs.Csd;
using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Domain.Entities.Csd;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;

namespace iTender.Integrator.Infrastructure.Integrations.CSD
{
    public class CsdApiClient : ICsdApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly CsdApiOptions _options;

        private AuthenticationResponse? _authentication;

        public CsdApiClient(
            HttpClient httpClient,
            IOptions<CsdApiOptions> options)
        {
            _httpClient = httpClient;
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

            await EnsureAuthenticatedAsync(cancellationToken);

            var request = new GetSupplierDetailRequest
            {
                SupplierNumber = supplierNumber
            };

            var xml = Serialize(request);

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                _options.SupplierDetailsEndpoint);

            httpRequest.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    _authentication!.Token.ToString());

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

            return Deserialize<CsdSupplier>(responseXml);
        }

        private async Task EnsureAuthenticatedAsync(
            CancellationToken cancellationToken)
        {
            if (_authentication is not null &&
                _authentication.TokenExpireDateTime > DateTime.Now.AddMinutes(1))
            {
                return;
            }

            var authenticationRequest = new AuthenticationRequest
            {
                AcceptTermsandConditions = true,
                Email = _options.Username,
                Password = _options.Password
            };

            var xml = Serialize(authenticationRequest);

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                _options.AuthenticateEndpoint);

            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/xml"));

            request.Content = new StringContent(
                xml,
                Encoding.UTF8,
                "application/xml");

            using var response = await _httpClient.SendAsync(
                request,
                cancellationToken);

            var responseXml = await response.Content
                 .ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"CSD authentication failed. " +
                    $"Status: {(int)response.StatusCode} {response.StatusCode}. " +
                    $"Response: {responseXml}");
            }

            _authentication =
                Deserialize<AuthenticationResponse>(responseXml);

            ValidateAuthentication();
        }

        private void ValidateAuthentication()
        {
            if (_authentication is null)
            {
                throw new InvalidOperationException(
                    "CSD authentication returned no response.");
            }

            if (_authentication.IsSuspended)
            {
                throw new InvalidOperationException(
                    "CSD account is suspended.");
            }

            if (_authentication.LockoutEnabled)
            {
                throw new InvalidOperationException(
                    "CSD account is locked.");
            }

            if (!_authentication.IsAccountActive)
            {
                throw new InvalidOperationException(
                    "CSD account is inactive.");
            }

            if (!_authentication.IsAccountVerified)
            {
                throw new InvalidOperationException(
                    "CSD account has not been verified.");
            }

            if (_authentication.IsPasswordExpired)
            {
                throw new InvalidOperationException(
                    "CSD account password has expired.");
            }

            if (_authentication.Token == Guid.Empty)
            {
                throw new InvalidOperationException(
                    "CSD authentication returned an invalid token.");
            }
        }

        private static string Serialize<T>(T value)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var writer = new Utf8StringWriter();
            serializer.Serialize(writer, value);
            return writer.ToString();
        }

        private static T Deserialize<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));

            using var reader = new StringReader(xml);

            return (T)serializer.Deserialize(reader)!;
        }
    }

    public sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }

}
