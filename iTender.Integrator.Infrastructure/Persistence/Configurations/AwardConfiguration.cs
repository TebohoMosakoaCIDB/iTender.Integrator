using iTender.Integrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iTender.Integrator.Infrastructure.Persistence.Configurations
{
    public class AwardConfiguration : IEntityTypeConfiguration<Award>
    {
        public void Configure(EntityTypeBuilder<Award> builder)
        {
            builder.ToTable("Awards");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.ExternalId).IsRequired().HasMaxLength(200);
            builder.Property(a => a.Title).HasMaxLength(500);
            builder.Property(a => a.Description).HasMaxLength(2000);

            builder.OwnsOne(a => a.Value, v =>
            {
                v.Property(m => m.Amount).HasColumnName("Value_Amount").HasColumnType("decimal(18,2)");
                v.Property(m => m.Currency).HasColumnName("Value_Currency").HasMaxLength(10);
            });

            // AwardSupplier has no Guid id of its own (ValueObject, not Entity) - an
            // owned collection gets a synthetic composite key (owner id + index),
            // which is fine here since suppliers are only ever read as a whole list
            // alongside their Award, never queried independently.
            builder.OwnsMany(a => a.Suppliers, s =>
            {
                s.WithOwner().HasForeignKey("AwardId");
                s.Property(x => x.ExternalId).HasColumnName("ExternalId").IsRequired().HasMaxLength(200);
                s.Property(x => x.Name).HasColumnName("Name").IsRequired().HasMaxLength(500);
                s.ToTable("AwardSuppliers");
            });
            builder.Navigation(a => a.Suppliers).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(a => a.DomainEvents);
        }
    }
}
