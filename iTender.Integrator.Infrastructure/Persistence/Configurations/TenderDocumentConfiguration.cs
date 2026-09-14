using iTender.Integrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iTender.Integrator.Infrastructure.Persistence.Configurations
{
    public class TenderDocumentConfiguration : IEntityTypeConfiguration<TenderDocument>
    {
        public void Configure(EntityTypeBuilder<TenderDocument> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.ExternalId).IsRequired().HasMaxLength(200);
            builder.Property(d => d.Url).IsRequired().HasMaxLength(1000);
            builder.Property(d => d.DocumentType).HasMaxLength(50);
            builder.Property(d => d.Title).HasMaxLength(500);
            builder.Property(d => d.Description).HasMaxLength(2000);
            builder.Property(d => d.Format).HasMaxLength(20);
            builder.Property(d => d.Language).HasMaxLength(10);

            builder.Ignore(d => d.DomainEvents);
        }
    }
}
