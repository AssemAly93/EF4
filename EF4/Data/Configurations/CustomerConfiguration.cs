using EF4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EF4.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.FullName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.NationalId).IsRequired().HasMaxLength(20).IsUnicode(false);
            builder.Property(c => c.DateOfBirth).HasColumnType("date");
            builder.Property(c => c.Email).IsRequired().HasMaxLength(100).IsUnicode(false);
            builder.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(20).IsUnicode(false);
            builder.Property(c => c.Address).IsRequired().HasMaxLength(200);

            builder.Property(c => c.CustomerType)
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.HasIndex(c => c.NationalId).IsUnique();
        }
    }
}
