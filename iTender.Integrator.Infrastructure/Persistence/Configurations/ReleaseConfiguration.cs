using iTender.Integrator.Domain.Entities;
using iTender.Integrator.Infrastructure.Persistence.Conversions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iTender.Integrator.Infrastructure.Persistence.Configurations
{
    public class ReleaseConfiguration : IEntityTypeConfiguration<Release>
    {
        public void Configure(EntityTypeBuilder<Release> builder)
        {
            builder.ToTable("Releases");
            builder.HasKey(r => r.Id);

            // The natural key from the OCDS side - AddAsync/UpsertAsync both key off
            // this pair, and a release is immutable once published so this uniquely
            // identifies one row forever.
            builder.HasIndex(r => new { r.Ocid, r.ReleaseId }).IsUnique();

            builder.Property(r => r.Ocid).IsRequired().HasMaxLength(200);
            builder.Property(r => r.ReleaseId).IsRequired().HasMaxLength(200);
            builder.Property(r => r.Description).HasMaxLength(2000);
            builder.Property(r => r.InitiationType).HasMaxLength(50);
            builder.Property(r => r.Language).IsRequired().HasMaxLength(10);
            builder.Property(r => r.BuyerId).HasMaxLength(200);
            builder.Property(r => r.BuyerName).HasMaxLength(500);

            // Drives IReleaseRepository.GetUnsyncedAsync - unsynced means this column
            // is still null.
            builder.HasIndex(r => r.LastSyncedAtUtc);

            builder.Property(r => r.Tags)
                .HasField("_tags")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Tags")
                .HasConversion(StringListConversion.Converter, StringListConversion.Comparer);

            // One release has at most one tender (this feed is tender-initiated
            // releases only, per OcdsReleaseMapper) - optional one-to-one, shadow FK
            // lives on Tenders since Release is the aggregate root.
            builder.HasOne(r => r.Tender)
                .WithOne()
                .HasForeignKey<Tender>("ReleaseId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Parties)
                .WithOne()
                .HasForeignKey("ReleaseId")
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(r => r.Parties).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(r => r.Awards)
                .WithOne()
                .HasForeignKey("ReleaseId")
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(r => r.Awards).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(r => r.Contracts)
                .WithOne()
                .HasForeignKey("ReleaseId")
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(r => r.Contracts).UsePropertyAccessMode(PropertyAccessMode.Field);

            // Domain events are transient (in-memory notification only) - never
            // persisted, and EF should never try to discover/map this collection.
            builder.Ignore(r => r.DomainEvents);
        }
    }
}
