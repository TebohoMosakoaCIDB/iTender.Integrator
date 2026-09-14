using iTender.Integrator.Domain.Entities;
using iTender.Integrator.Infrastructure.Persistence.Conversions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iTender.Integrator.Infrastructure.Persistence.Configurations
{
    public class TenderConfiguration : IEntityTypeConfiguration<Tender>
    {
        public void Configure(EntityTypeBuilder<Tender> builder)
        {
            builder.ToTable("Tenders");
            builder.HasKey(t => t.Id);

            // Not globally unique on its own (it's an id scoped to one procuring
            // system) - global identity for a release is (Ocid, ReleaseId) on the
            // parent, this is just a lookup convenience.
            builder.HasIndex(t => t.ExternalId);

            builder.Property(t => t.ExternalId).IsRequired().HasMaxLength(200);
            builder.Property(t => t.Title).IsRequired().HasMaxLength(500);
            builder.Property(t => t.Category).HasMaxLength(200);
            builder.Property(t => t.MainProcurementCategory).HasMaxLength(50);
            builder.Property(t => t.Description).HasMaxLength(4000);
            builder.Property(t => t.Province).HasMaxLength(100);
            builder.Property(t => t.DeliveryLocation).HasMaxLength(500);
            builder.Property(t => t.EligibilityCriteria).HasMaxLength(2000);
            builder.Property(t => t.ProcurementMethod).HasMaxLength(50);
            builder.Property(t => t.ProcurementMethodDetails).HasMaxLength(500);
            builder.Property(t => t.ProcuringEntityId).HasMaxLength(200);
            builder.Property(t => t.ProcuringEntityName).HasMaxLength(500);

            builder.Property(t => t.AdditionalProcurementCategories)
                .HasField("_additionalProcurementCategories")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("AdditionalProcurementCategories")
                .HasConversion(StringListConversion.Converter, StringListConversion.Comparer);

            builder.OwnsOne(t => t.Value, v =>
            {
                v.Property(m => m.Amount).HasColumnName("Value_Amount").HasColumnType("decimal(18,2)");
                v.Property(m => m.Currency).HasColumnName("Value_Currency").HasMaxLength(10);
            });

            builder.OwnsOne(t => t.TenderPeriod, p =>
            {
                p.Property(d => d.StartDate).HasColumnName("TenderPeriod_StartDate");
                p.Property(d => d.EndDate).HasColumnName("TenderPeriod_EndDate");
                p.Property(d => d.MaxExtentDate).HasColumnName("TenderPeriod_MaxExtentDate");
                p.Property(d => d.DurationInDays).HasColumnName("TenderPeriod_DurationInDays");
            });

            builder.OwnsOne(t => t.EnquiryPeriod, p =>
            {
                p.Property(d => d.StartDate).HasColumnName("EnquiryPeriod_StartDate");
                p.Property(d => d.EndDate).HasColumnName("EnquiryPeriod_EndDate");
                p.Property(d => d.MaxExtentDate).HasColumnName("EnquiryPeriod_MaxExtentDate");
                p.Property(d => d.DurationInDays).HasColumnName("EnquiryPeriod_DurationInDays");
            });

            builder.OwnsOne(t => t.AwardPeriod, p =>
            {
                p.Property(d => d.StartDate).HasColumnName("AwardPeriod_StartDate");
                p.Property(d => d.EndDate).HasColumnName("AwardPeriod_EndDate");
                p.Property(d => d.MaxExtentDate).HasColumnName("AwardPeriod_MaxExtentDate");
                p.Property(d => d.DurationInDays).HasColumnName("AwardPeriod_DurationInDays");
            });

            builder.OwnsOne(t => t.ContactPerson, c =>
            {
                c.Property(x => x.Name).HasColumnName("ContactPerson_Name").HasMaxLength(200);
                c.Property(x => x.Telephone).HasColumnName("ContactPerson_Telephone").HasMaxLength(50);
                c.Property(x => x.Email).HasColumnName("ContactPerson_Email").HasMaxLength(200);
                c.Property(x => x.FaxNumber).HasColumnName("ContactPerson_FaxNumber").HasMaxLength(50);
                c.Property(x => x.Url).HasColumnName("ContactPerson_Url").HasMaxLength(500);
            });

            builder.OwnsOne(t => t.BriefingSession, b =>
            {
                b.Property(x => x.IsSession).HasColumnName("BriefingSession_IsSession");
                b.Property(x => x.Compulsory).HasColumnName("BriefingSession_Compulsory");
                b.Property(x => x.Date).HasColumnName("BriefingSession_Date");
                b.Property(x => x.Venue).HasColumnName("BriefingSession_Venue").HasMaxLength(500);
            });

            builder.OwnsOne(t => t.Classification, c =>
            {
                c.Property(x => x.Scheme).HasColumnName("Classification_Scheme").HasMaxLength(50);
                c.Property(x => x.Code).HasColumnName("Classification_Code").HasMaxLength(50);
                c.Property(x => x.Description).HasColumnName("Classification_Description").HasMaxLength(500);
            });

            builder.HasMany(t => t.Lots)
                .WithOne()
                .HasForeignKey("TenderId")
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(t => t.Lots).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(t => t.Items)
                .WithOne()
                .HasForeignKey("TenderId")
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(t => t.Items).UsePropertyAccessMode(PropertyAccessMode.Field);

            // TenderDocument is also used by Contract.Documents - see
            // TenderDocumentConfiguration for why both relationships share one table.
            builder.HasMany(t => t.Documents)
                .WithOne()
                .HasForeignKey("TenderId")
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(t => t.Documents).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(t => t.DomainEvents);
        }
    }

    public class LotConfiguration : IEntityTypeConfiguration<Lot>
    {
        public void Configure(EntityTypeBuilder<Lot> builder)
        {
            builder.ToTable("Lots");
            builder.HasKey(l => l.Id);

            builder.Property(l => l.ExternalId).IsRequired().HasMaxLength(200);
            builder.Property(l => l.Description).HasMaxLength(2000);
            builder.Property(l => l.Status).HasMaxLength(50);

            builder.OwnsOne(l => l.Value, v =>
            {
                v.Property(m => m.Amount).HasColumnName("Value_Amount").HasColumnType("decimal(18,2)");
                v.Property(m => m.Currency).HasColumnName("Value_Currency").HasMaxLength(10);
            });

            builder.OwnsOne(l => l.ContractPeriod, p =>
            {
                p.Property(d => d.StartDate).HasColumnName("ContractPeriod_StartDate");
                p.Property(d => d.EndDate).HasColumnName("ContractPeriod_EndDate");
                p.Property(d => d.MaxExtentDate).HasColumnName("ContractPeriod_MaxExtentDate");
                p.Property(d => d.DurationInDays).HasColumnName("ContractPeriod_DurationInDays");
            });

            builder.Ignore(l => l.DomainEvents);
        }
    }

    public class TenderItemConfiguration : IEntityTypeConfiguration<TenderItem>
    {
        public void Configure(EntityTypeBuilder<TenderItem> builder)
        {
            builder.ToTable("TenderItems");
            builder.HasKey(i => i.Id);

            builder.Property(i => i.ExternalId).IsRequired().HasMaxLength(200);
            builder.Property(i => i.Description).HasMaxLength(2000);
            builder.Property(i => i.Quantity).HasColumnType("decimal(18,4)");
            builder.Property(i => i.Unit).HasMaxLength(50);

            builder.OwnsOne(i => i.Classification, c =>
            {
                c.Property(x => x.Scheme).HasColumnName("Classification_Scheme").HasMaxLength(50);
                c.Property(x => x.Code).HasColumnName("Classification_Code").HasMaxLength(50);
                c.Property(x => x.Description).HasColumnName("Classification_Description").HasMaxLength(500);
            });

            builder.Ignore(i => i.DomainEvents);
        }
    }
}
