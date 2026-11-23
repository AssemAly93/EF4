using EF4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EF4.Data.Configurations
{
    public class ManagerConfiguration : IEntityTypeConfiguration<Manager>
    {
        public void Configure(EntityTypeBuilder<Manager> builder)
        {
            builder.ToTable("Managers");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.FullName).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Email).IsRequired().HasMaxLength(100).IsUnicode(false);
            builder.Property(m => m.PhoneNumber).IsRequired().HasMaxLength(20).IsUnicode(false);
            builder.Property(m => m.HireDate).HasColumnType("date");

            builder.Property(m => m.BranchCode).IsRequired().HasMaxLength(10).IsUnicode(false);
            builder.HasIndex(m => m.BranchCode).IsUnique();

            builder.HasIndex(m => m.Email).IsUnique();
        }
    }
}
