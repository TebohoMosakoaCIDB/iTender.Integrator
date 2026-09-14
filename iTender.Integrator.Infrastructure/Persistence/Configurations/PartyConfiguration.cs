using iTender.Integrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iTender.Integrator.Infrastructure.Persistence.Configurations
{
    public class PartyConfiguration : IEntityTypeConfiguration<Party>
    {
        public void Configure(EntityTypeBuilder<Party> builder)
        {
            builder.ToTable("Parties");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.ExternalId).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(500);
            builder.Property(p => p.LegalName).HasMaxLength(500);
            builder.Property(p => p.RegistrationScheme).HasMaxLength(50);
            builder.Property(p => p.RegistrationNumber).HasMaxLength(100);

            // Used to join CSD/CRM compliance results back to a party on read.
            builder.HasIndex(p => p.RegistrationNumber);

            // Roles is a [Flags]-style enum (bitwise) - stored as its underlying int
            // by EF's default enum conversion, which is exactly right for a bitmask.

            builder.OwnsOne(p => p.Address, a =>
            {
                a.Property(x => x.StreetAddress).HasColumnName("Address_StreetAddress").HasMaxLength(500);
                a.Property(x => x.Locality).HasColumnName("Address_Locality").HasMaxLength(200);
                a.Property(x => x.Region).HasColumnName("Address_Region").HasMaxLength(200);
                a.Property(x => x.PostalCode).HasColumnName("Address_PostalCode").HasMaxLength(20);
                a.Property(x => x.CountryName).HasColumnName("Address_CountryName").HasMaxLength(100);
            });

            builder.OwnsOne(p => p.ContactPoint, c =>
            {
                c.Property(x => x.Name).HasColumnName("ContactPoint_Name").HasMaxLength(200);
                c.Property(x => x.Telephone).HasColumnName("ContactPoint_Telephone").HasMaxLength(50);
                c.Property(x => x.Email).HasColumnName("ContactPoint_Email").HasMaxLength(200);
                c.Property(x => x.FaxNumber).HasColumnName("ContactPoint_FaxNumber").HasMaxLength(50);
                c.Property(x => x.Url).HasColumnName("ContactPoint_Url").HasMaxLength(500);
            });

            builder.Ignore(p => p.DomainEvents);
        }
    }
}
