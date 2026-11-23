using EF4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EF4.Data.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");

            builder.HasKey(t => t.TransactionNumber);
            builder.Property(t => t.TransactionNumber).UseIdentityColumn(1000, 1);

            builder.Property(t => t.TransactionDate).HasColumnType("datetime2");
            builder.Property(t => t.Amount).HasPrecision(18, 2);

            builder.Property(t => t.TransactionType)
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(t => t.Note).HasMaxLength(250);

            builder.Property(t => t.AccountNumber).IsRequired().HasMaxLength(20).IsUnicode(false);
        }
    }
}
