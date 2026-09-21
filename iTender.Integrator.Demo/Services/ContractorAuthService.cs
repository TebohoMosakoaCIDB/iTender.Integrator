using iTender.Integrator.Application.DTOs.Compliance;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net;
using System.Text.Json;

namespace iTender.Integrator.Demo.Services
{
    public sealed class ContractorAuthService
    {
        private const string StorageKey = "ibbidder.session";

        private readonly IntegratorApiClient _api;
        private readonly ProtectedSessionStorage _session;
        private readonly ILogger<ContractorAuthService> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ContractorAuthService(
            IntegratorApiClient api,
            ProtectedSessionStorage session,
            ILogger<ContractorAuthService> logger)
        {
            _api = api;
            _session = session;
            _logger = logger;
        }

        /// <summary>
        /// Currently signed-in contractor, if any.
        /// </summary>
        public ContractorSession? Current { get; private set; }

        public event Action? OnChange;

        public bool IsAuthenticated => Current is not null;

        /// <summary>
        /// Attempts to sign in with a CRS number.
        /// Returns a result with a status the UI can branch on.
        /// </summary>
        public async Task<LoginResult> LoginAsync(
            string crsNumber,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(crsNumber))
            {
                return LoginResult.Fail("Please enter your CRS number.");
            }

            crsNumber = crsNumber.Trim();

            JsonElement element;
            try
            {
                element = await _api.GetContractorAsync(crsNumber, cancellationToken);
            }
            catch (IntegratorApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return LoginResult.Fail(
                    "No contractor found with that CRS number. Please check and try again.");
            }
            catch (IntegratorApiException ex)
            {
                _logger.LogError(ex,
                    "API error during login for CRS {CrsNumber}", crsNumber);
                return LoginResult.Fail(
                    $"Could not reach the contractor service ({(int)ex.StatusCode}).");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unexpected error during login for CRS {CrsNumber}", crsNumber);
                return LoginResult.Fail("Unexpected error. Please try again.");
            }

            ContractorFullProfile? profile;
            try
            {
                profile = element.Deserialize<ContractorFullProfile>(JsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize contractor profile.");
                return LoginResult.Fail("Unexpected response from the server.");
            }

            if (profile is null || profile.Crm is null)
            {
                return LoginResult.Fail("Your contractor record could not be loaded.");
            }

            // Optional gate — uncomment to block sanctioned contractors.
            //if (profile.Crm.IsSanctioned)
            //{
            //    return LoginResult.Fail(
            //        "Your company is currently sanctioned and cannot access the portal.");
            //}

            var session = new ContractorSession
            {
                CrsNumber = profile.CrsNumber,
                ContractorId = profile.Crm.Id,
                DisplayName = profile.Crm.TradingAs
                    ?? profile.Crm.EnterpriseName
                    ?? profile.Crm.Name
                    ?? $"Contractor {profile.CrsNumber}",
                SignedInAtUtc = DateTime.UtcNow
            };

            await _session.SetAsync(StorageKey, session);
            Current = session;
            OnChange?.Invoke();

            return LoginResult.Ok(session);
        }

        /// <summary>
        /// Rehydrates the session on app start (or after a refresh).
        /// </summary>
        public async Task RestoreAsync()
        {
            try
            {
                var result = await _session.GetAsync<ContractorSession>(StorageKey);
                if (result.Success && result.Value is not null)
                {
                    Current = result.Value;
                    OnChange?.Invoke();
                }
            }
            catch
            {
                // Browser storage not available (e.g. pre-render). Ignore.
            }
        }

        public async Task LogoutAsync()
        {
            try { await _session.DeleteAsync(StorageKey); }
            catch { /* ignore */ }

            Current = null;
            OnChange?.Invoke();
        }
    }

    public sealed class ContractorSession
    {
        public string CrsNumber { get; set; } = string.Empty;
        public Guid ContractorId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public DateTime SignedInAtUtc { get; set; }
    }

    public sealed record LoginResult(
        bool Success,
        string? ErrorMessage,
        ContractorSession? Session)
    {
        public static LoginResult Ok(ContractorSession s) => new(true, null, s);
        public static LoginResult Fail(string message) => new(false, message, null);
    }
}
