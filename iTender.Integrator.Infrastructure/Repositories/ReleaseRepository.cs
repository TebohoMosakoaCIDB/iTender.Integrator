using iTender.Integrator.Application.Interfaces;
using iTender.Integrator.Domain.Entities;
using iTender.Integrator.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace iTender.Integrator.Infrastructure.Repositories
{
    public class ReleaseRepository : IReleaseRepository
    {
        private readonly ItenderIntegratorDbContext _db;

        public ReleaseRepository(ItenderIntegratorDbContext db)
        {
            _db = db;
        }

        public Task<Release?> GetByOcidAndReleaseIdAsync(
            string ocid, string releaseId, CancellationToken cancellationToken = default)
            => FullGraph()
                .FirstOrDefaultAsync(r => r.Ocid == ocid && r.ReleaseId == releaseId, cancellationToken);

        public Task<Release?> GetLatestByOcidAsync(string ocid, CancellationToken cancellationToken = default)
            => FullGraph()
                .Where(r => r.Ocid == ocid)
                .OrderByDescending(r => r.ReleaseDate)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task AddAsync(Release release, CancellationToken cancellationToken = default)
        {
            await _db.Releases.AddAsync(release, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Release release, CancellationToken cancellationToken = default)
        {
            // Release is loaded fresh per request in this codebase (no long-lived
            // context), so Update() (mark-all-modified) is the pragmatic choice here
            // rather than relying on change tracking across a call boundary the
            // caller doesn't control.
            _db.Releases.Update(release);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<Release>> GetUnsyncedAsync(
            int take = 100, CancellationToken cancellationToken = default)
        {
            var releases = await FullGraph()
                .Where(r => r.LastSyncedAtUtc == null)
                .OrderBy(r => r.FetchedAtUtc)
                .Take(take)
                .ToListAsync(cancellationToken);

            return releases;
        }

        public Task<DateTime?> GetLatestFetchedAtUtcAsync(CancellationToken cancellationToken = default)
            => _db.Releases.MaxAsync(r => (DateTime?)r.FetchedAtUtc, cancellationToken);

        // Centralizes the eager-loading needed to reconstitute a full aggregate -
        // every read path needs the same graph, so callers never get a
        // partially-loaded Release back.
        private IQueryable<Release> FullGraph()
            => _db.Releases
                .Include(r => r.Tender!).ThenInclude(t => t.Lots)
                .Include(r => r.Tender!).ThenInclude(t => t.Items)
                .Include(r => r.Tender!).ThenInclude(t => t.Documents)
                .Include(r => r.Parties)
                .Include(r => r.Awards).ThenInclude(a => a.Suppliers)
                .Include(r => r.Contracts).ThenInclude(c => c.Milestones)
                .Include(r => r.Contracts).ThenInclude(c => c.Transactions)
                .Include(r => r.Contracts).ThenInclude(c => c.Documents);
    }
}
