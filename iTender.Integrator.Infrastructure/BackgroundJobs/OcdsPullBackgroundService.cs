using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace iTender.Integrator.Infrastructure.BackgroundJobs
{
    public class OcdsPullBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly OcdsPullOptions _options;
        private readonly ILogger<OcdsPullBackgroundService> _logger;

        private DateTime? _lastSuccessfulPullUtc;

        public OcdsPullBackgroundService(
            IServiceScopeFactory scopeFactory,
            IOptions<OcdsPullOptions> options,
            ILogger<OcdsPullBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_options.Enabled)
            {
                _logger.LogInformation("OCDS pull background service is disabled (OcdsPull:Enabled=false).");
                return;
            }

            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(Math.Max(1, _options.IntervalMinutes)));

            // Run once immediately on startup, then again on every tick.
            do
            {
                await RunOnceAsync(stoppingToken);
            }
            while (!stoppingToken.IsCancellationRequested
                && await timer.WaitForNextTickAsync(stoppingToken));
        }

        private async Task RunOnceAsync(CancellationToken stoppingToken)
        {
            var runStartedUtc = DateTime.UtcNow;

            // OcdsPullBackgroundService itself is a singleton (required for
            // BackgroundService); IOcdsApiClient/IReleaseComplianceService/
            // IReleaseRepository sit on top of scoped dependencies (ICrmServiceFactory,
            // HttpClient, the DbContext), so each run gets its own DI scope rather
            // than capturing scoped services at construction time.
            using var scope = _scopeFactory.CreateScope();
            var ocdsApiClient = scope.ServiceProvider.GetRequiredService<IOcdsApiClient>();
            var releaseComplianceService = scope.ServiceProvider.GetRequiredService<IReleaseComplianceService>();
            var releaseRepository = scope.ServiceProvider.GetRequiredService<IReleaseRepository>();

            var from = _lastSuccessfulPullUtc;

            if (from is null)
            {
                // Cold start (or first run after a restart) - ask the database for
                // the last time we actually persisted something, rather than
                // assuming there's nothing there. This is what makes the in-memory
                // fast path safe to lose on restart: it's a cache of this value, not
                // the only copy of it.
                from = await releaseRepository.GetLatestFetchedAtUtcAsync(stoppingToken);

                if (from.HasValue)
                {
                    _logger.LogInformation(
                        "No in-memory cursor (likely a fresh start) - resuming from persisted cursor {From:o}.", from);
                }
            }

            from ??= runStartedUtc.AddHours(-Math.Max(1, _options.InitialLookbackHours));

            _logger.LogInformation("OCDS pull run starting - from {From:o} to {To:o}.", from, runStartedUtc);

            var pulled = 0;
            var published = 0;
            var notPublished = 0;
            var complianceFlags = 0;
            var pageNumber = 1;

            try
            {
                while (pageNumber <= Math.Max(1, _options.MaxPagesPerRun))
                {
                    var package = await ocdsApiClient.GetReleasesAsync(
                        pageNumber, _options.PageSize, from, runStartedUtc, stoppingToken);

                    if (package.Releases.Count == 0)
                        break;

                    pulled += package.Releases.Count;

                    var views = await releaseComplianceService.EnrichAsync(package.Releases, stoppingToken);

                    foreach (var view in views)
                    {
                        if (view.PublishedToCrm) published++;
                        else notPublished++;

                        complianceFlags += view.Parties.Count(p =>
                            p.ComplianceStatus is CidbComplianceStatus.NonCompliant
                                or CidbComplianceStatus.RegistrationSuspended
                                or CidbComplianceStatus.NotChecked);
                    }

                    if (package.Releases.Count < _options.PageSize)
                        break;

                    pageNumber++;
                }

                // Only advance the cursor on a fully successful run - see the catch
                // block below for why a failed run must not lose its window.
                _lastSuccessfulPullUtc = runStartedUtc;

                _logger.LogInformation(
                    "OCDS pull run complete - {Pulled} releases pulled, {Published} published to CRM, " +
                    "{NotPublished} not published (skipped/no tender/failed), {ComplianceFlags} party compliance flags raised.",
                    pulled, published, notPublished, complianceFlags);
            }
            catch (Exception ex)
            {
                // Deliberately NOT advancing _lastSuccessfulPullUtc here - a failed
                // run (e.g. NT eTender or CRM outage mid-pull) retries the same
                // [from, runStartedUtc) window next tick instead of silently
                // skipping whatever it didn't get to.
                _logger.LogError(
                    ex,
                    "OCDS pull run failed after {Pulled} releases - will retry from the same window ({From:o}) next run.",
                    pulled, from);
            }
        }
    }
}
