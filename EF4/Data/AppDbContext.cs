using EF4.Data.Seeding;
using EF4.Models;
using Microsoft.EntityFrameworkCore;

namespace EF4.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Branch> Branches { get; set; } = null!;
        public DbSet<Manager> Managers { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<CustomerAccount> CustomerAccounts { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=.;Database=NationalBankDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.Seed();
        }
    }
}
