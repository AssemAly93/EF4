using EF4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EF4.Data.Configurations
{
    public class CustomerAccountConfiguration : IEntityTypeConfiguration<CustomerAccount>
    {
        public void Configure(EntityTypeBuilder<CustomerAccount> builder)
        {
            builder.ToTable("CustomerAccounts");

            builder.HasKey(ca => new { ca.CustomerId, ca.AccountNumber });

            builder.Property(ca => ca.AccountNumber).HasMaxLength(20).IsUnicode(false);

            builder.Property(ca => ca.OwnershipType)
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(ca => ca.AccountStatus)
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(ca => ca.OwnershipStartDate).HasColumnType("date");

            builder.HasOne(ca => ca.Customer)
                   .WithMany(c => c.CustomerAccounts)
                   .HasForeignKey(ca => ca.CustomerId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ca => ca.Account)
                   .WithMany(a => a.CustomerAccounts)
                   .HasForeignKey(ca => ca.AccountNumber)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
