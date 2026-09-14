using iTender.Integrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iTender.Integrator.Infrastructure.Persistence.Configurations
{
    public class ContractConfiguration : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            builder.ToTable("Contracts");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.ExternalId).IsRequired().HasMaxLength(200);
            builder.Property(c => c.AwardExternalId).HasMaxLength(200);
            builder.Property(c => c.Title).HasMaxLength(500);
            builder.Property(c => c.Description).HasMaxLength(2000);

            // Point #5 (Contract.HasNoLinkedAward) reads on this - not a real FK
            // relationship since awards/contracts are correlated by OCDS's own
            // external id, not our internal Guid keys.
            builder.HasIndex(c => c.AwardExternalId);

            builder.OwnsOne(c => c.Value, v =>
            {
                v.Property(m => m.Amount).HasColumnName("Value_Amount").HasColumnType("decimal(18,2)");
                v.Property(m => m.Currency).HasColumnName("Value_Currency").HasMaxLength(10);
            });

            builder.OwnsOne(c => c.Period, p =>
            {
                p.Property(d => d.StartDate).HasColumnName("Period_StartDate");
                p.Property(d => d.EndDate).HasColumnName("Period_EndDate");
                p.Property(d => d.MaxExtentDate).HasColumnName("Period_MaxExtentDate");
                p.Property(d => d.DurationInDays).HasColumnName("Period_DurationInDays");
            });

            builder.HasMany(c => c.Milestones)
                .WithOne()
                .HasForeignKey("ContractId")
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(c => c.Milestones).UsePropertyAccessMode(PropertyAccessMode.Field);

            // ContractTransaction has no identity of its own (ValueObject) - owned
            // collection with a synthetic key, same reasoning as AwardSupplier.
            builder.OwnsMany(c => c.Transactions, t =>
            {
                t.WithOwner().HasForeignKey("ContractId");
                t.Property(x => x.ExternalId).IsRequired().HasMaxLength(200);
                t.Property(x => x.Date);
                t.Property(x => x.PayerId).HasMaxLength(200);
                t.Property(x => x.PayeeId).HasMaxLength(200);
                t.OwnsOne(x => x.Value, v =>
                {
                    v.Property(m => m.Amount).HasColumnName("Value_Amount").HasColumnType("decimal(18,2)");
                    v.Property(m => m.Currency).HasColumnName("Value_Currency").HasMaxLength(10);
                });
                t.ToTable("ContractTransactions");
            });
            builder.Navigation(c => c.Transactions).UsePropertyAccessMode(PropertyAccessMode.Field);

            // Shares the TenderDocuments table with Tender.Documents - see
            // TenderDocumentConfiguration.
            builder.HasMany(c => c.Documents)
                .WithOne()
                .HasForeignKey("ContractId")
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(c => c.Documents).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(c => c.DomainEvents);
        }
    }

    public class ContractMilestoneConfiguration : IEntityTypeConfiguration<ContractMilestone>
    {
        public void Configure(EntityTypeBuilder<ContractMilestone> builder)
        {
            builder.ToTable("ContractMilestones");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.ExternalId).IsRequired().HasMaxLength(200);
            builder.Property(m => m.Title).HasMaxLength(500);
            builder.Property(m => m.Type).HasMaxLength(50);

            builder.Ignore(m => m.DomainEvents);
        }
    }
}
