using iTender.Integrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace iTender.Integrator.Infrastructure.Persistence
{
    public class ItenderIntegratorDbContext : DbContext
    {
        public ItenderIntegratorDbContext(DbContextOptions<ItenderIntegratorDbContext> options)
            : base(options)
        {
        }

        public DbSet<Release> Releases => Set<Release>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ItenderIntegratorDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
