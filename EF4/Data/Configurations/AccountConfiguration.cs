using EF4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EF4.Data.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");

            builder.HasKey(a => a.AccountNumber);
            builder.Property(a => a.AccountNumber).HasMaxLength(20).IsUnicode(false);

            builder.Property(a => a.AccountType)
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(a => a.CurrentBalance).HasPrecision(18, 2);

            builder.Property(a => a.OpeningDate).HasColumnType("date");

            builder.Property(a => a.BranchCode).IsRequired().HasMaxLength(10).IsUnicode(false);

            builder.HasMany(a => a.Transactions)
                   .WithOne(t => t.Account)
                   .HasForeignKey(t => t.AccountNumber)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
