using iTender.Integrator.Application.DTOs.ETenders;
using iTender.Integrator.Application.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace iTender.Integrator.Infrastructure.Integrations.eTenders
{
    public class ETendersAdminApiClient : IETendersAdminApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ETendersAdminApiOptions _options;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ETendersAdminApiClient(HttpClient httpClient, IOptions<ETendersAdminApiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<EtendersApiResponse> CreateTenderAsync(
            CreateTenderRequest request,
            IReadOnlyCollection<TenderDocumentUpload>? documents = null,
            CancellationToken cancellationToken = default)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            using var content = BuildMultipartContent(request, documents);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, _options.CreateTenderEndpoint)
            {
                Content = content
            };

            // UNCONFIRMED auth mechanism - see ETendersAdminApiOptions. This is a
            // no-op until ApiKeyHeaderName/ApiKeyValue are actually configured.
            if (!string.IsNullOrWhiteSpace(_options.ApiKeyHeaderName) && !string.IsNullOrWhiteSpace(_options.ApiKeyValue))
                httpRequest.Headers.Add(_options.ApiKeyHeaderName, _options.ApiKeyValue);

            using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                // Try to parse the real ApiResponse error shape first (the spec's
                // 400/404/409/500 responses all use it) - fall back to the raw body
                // if that fails, rather than losing the actual error detail.
                var parsed = TryParse(responseBody);
                if (parsed is not null)
                    return parsed;

                throw new HttpRequestException(
                    $"eTenders create-tender request failed. Status: {(int)response.StatusCode} {response.StatusCode}. " +
                    $"Response: {responseBody}");
            }

            return TryParse(responseBody)
                ?? throw new InvalidOperationException(
                    $"eTenders returned a success status but the response body couldn't be parsed: {responseBody}");
        }

        private static EtendersApiResponse? TryParse(string body)
        {
            if (string.IsNullOrWhiteSpace(body)) return null;

            try
            {
                return JsonSerializer.Deserialize<EtendersApiResponse>(body, JsonOptions);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private static MultipartFormDataContent BuildMultipartContent(
            CreateTenderRequest request, IReadOnlyCollection<TenderDocumentUpload>? documents)
        {
            var content = new MultipartFormDataContent();

            AddRequired(content, "Username", request.Username);
            AddRequired(content, "TenderNumber", request.TenderNumber);
            AddRequired(content, "PublishedDate", request.PublishedDate);
            AddRequired(content, "ClosingDate", request.ClosingDate);
            AddRequired(content, "TenderStatus", request.TenderStatus);
            AddRequired(content, "TenderCategoryId", request.TenderCategoryId);
            AddRequired(content, "CategoriesID", request.CategoriesID);
            AddRequired(content, "OrganOfStateId", request.OrganOfStateId);
            AddRequired(content, "ProvinceId", request.ProvinceId);
            AddRequired(content, "ProcurementPlan", request.ProcurementPlan);
            AddRequired(content, "Description", request.Description);
            AddRequired(content, "TenderTypeId", request.TenderTypeId);
            AddRequired(content, "ValidityStartDate", request.ValidityStartDate);
            AddRequired(content, "ValidityEndDate", request.ValidityEndDate);
            AddRequired(content, "BriefingSession", request.BriefingSession);
            AddRequired(content, "IsBriefingSessionCompulsory", request.IsBriefingSessionCompulsory);
            AddRequired(content, "StreetName", request.StreetName);
            AddRequired(content, "Surburb", request.Surburb);
            AddRequired(content, "Town", request.Town);
            AddRequired(content, "Code", request.Code);
            AddRequired(content, "ContactPerson", request.ContactPerson);
            AddRequired(content, "Email", request.Email);
            AddRequired(content, "Telephone", request.Telephone);
            AddRequired(content, "ESubmission", request.ESubmission);

            AddOptional(content, "ProcurementId", request.ProcurementId);
            AddOptional(content, "TenderSubTypeId", request.TenderSubTypeId);
            AddOptional(content, "BriefingVenue", request.BriefingVenue);
            AddOptional(content, "BriefingSessionDate", request.BriefingSessionDate);
            AddOptional(content, "SpecialConditions", request.SpecialConditions);
            AddOptional(content, "CompanyProfile", request.CompanyProfile);
            AddOptional(content, "IsCompanyProfileMandatory", request.IsCompanyProfileMandatory);
            AddOptional(content, "ProposalDescriptionOfSolution", request.ProposalDescriptionOfSolution);
            AddOptional(content, "IsProposalDescriptionOfSolutionMandatory", request.IsProposalDescriptionOfSolutionMandatory);
            AddOptional(content, "HighLevelExecutionPlan", request.HighLevelExecutionPlan);
            AddOptional(content, "IsHighLevelExecutionPlanMandatory", request.IsHighLevelExecutionPlanMandatory);
            AddOptional(content, "PricingSchedule", request.PricingSchedule);
            AddOptional(content, "IsPricingScheduleMandatory", request.IsPricingScheduleMandatory);
            AddOptional(content, "BillingSchedule", request.BillingSchedule);
            AddOptional(content, "IsBillingScheduleMandatory", request.IsBillingScheduleMandatory);
            AddOptional(content, "Quote", request.Quote);
            AddOptional(content, "IsQuoteMandatory", request.IsQuoteMandatory);
            AddOptional(content, "PricingToBeSubmittedInASeparateSealedEnvelope", request.PricingToBeSubmittedInASeparateSealedEnvelope);

            if (request.Questions is not null)
                foreach (var q in request.Questions)
                    AddRequired(content, "Questions", q);

            if (request.Answer is not null)
                foreach (var a in request.Answer)
                    AddRequired(content, "Answer", a);

            if (documents is not null)
            {
                foreach (var doc in documents)
                {
                    var streamContent = new StreamContent(doc.Content);
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(
                        string.IsNullOrWhiteSpace(doc.ContentType) ? "application/octet-stream" : doc.ContentType);
                    content.Add(streamContent, "Documents", doc.FileName);
                }
            }

            return content;
        }

        // Dates as round-trip ISO 8601, bools as lowercase "true"/"false", enums by
        // name - all match how ASP.NET Core model-binds a multipart form back into
        // the same shapes on the receiving end.
        private static void AddRequired(MultipartFormDataContent content, string name, string value)
            => content.Add(new StringContent(value), name);

        private static void AddRequired(MultipartFormDataContent content, string name, DateTime value)
            => content.Add(new StringContent(value.ToString("O")), name);

        private static void AddRequired(MultipartFormDataContent content, string name, bool value)
            => content.Add(new StringContent(value ? "true" : "false"), name);

        private static void AddRequired(MultipartFormDataContent content, string name, int value)
            => content.Add(new StringContent(value.ToString()), name);

        private static void AddRequired<TEnum>(MultipartFormDataContent content, string name, TEnum value)
            where TEnum : struct, Enum
            => content.Add(new StringContent(value.ToString()), name);

        private static void AddOptional(MultipartFormDataContent content, string name, string? value)
        {
            if (value is not null) content.Add(new StringContent(value), name);
        }

        private static void AddOptional(MultipartFormDataContent content, string name, DateTime? value)
        {
            if (value.HasValue) content.Add(new StringContent(value.Value.ToString("O")), name);
        }

        private static void AddOptional(MultipartFormDataContent content, string name, bool? value)
        {
            if (value.HasValue) content.Add(new StringContent(value.Value ? "true" : "false"), name);
        }

        private static void AddOptional(MultipartFormDataContent content, string name, int? value)
        {
            if (value.HasValue) content.Add(new StringContent(value.Value.ToString()), name);
        }
    }
}
