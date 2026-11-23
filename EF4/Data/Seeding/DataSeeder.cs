using EF4.Models;
using Microsoft.EntityFrameworkCore;

namespace EF4.Data.Seeding
{
    public static class DataSeeder
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Branch>().HasData(
                new Branch { Code = "CAI-01", Name = "Cairo Main Branch", Address = "15 Tahrir Square, Downtown, Cairo", PhoneNumber = "0225750000" },
                new Branch { Code = "ALX-01", Name = "Alexandria Branch", Address = "40 Fouad Street, Alexandria",        PhoneNumber = "0034870000" },
                new Branch { Code = "GIZ-01", Name = "Giza Branch",       Address = "22 Pyramids Road, Giza",             PhoneNumber = "0235720000" }
            );

            modelBuilder.Entity<Manager>().HasData(
                new Manager { Id = 1, FullName = "Hassan Mahmoud", Email = "hassan.mahmoud@nationalbank.eg", PhoneNumber = "01001234567", HireDate = new DateTime(2015, 3, 1),  BranchCode = "CAI-01" },
                new Manager { Id = 2, FullName = "Sara Kamel",     Email = "sara.kamel@nationalbank.eg",     PhoneNumber = "01201234567", HireDate = new DateTime(2018, 9, 15), BranchCode = "ALX-01" },
                new Manager { Id = 3, FullName = "Omar Fathy",     Email = "omar.fathy@nationalbank.eg",     PhoneNumber = "01101234567", HireDate = new DateTime(2020, 1, 10), BranchCode = "GIZ-01" }
            );

            modelBuilder.Entity<Customer>().HasData(
                new Customer { Id = 1, FullName = "Ahmed Ali",         NationalId = "29001011234567", DateOfBirth = new DateTime(1990, 1, 1),  Email = "ahmed.ali@gmail.com",   PhoneNumber = "01012345678", Address = "10 Nasr City, Cairo",        CustomerType = CustomerType.Individual },
                new Customer { Id = 2, FullName = "Nile Trading LLC",  NationalId = "50012345678",    DateOfBirth = new DateTime(2010, 6, 15), Email = "info@niletrading.com",  PhoneNumber = "0034567890",  Address = "5 Corniche Road, Alexandria", CustomerType = CustomerType.Business }
            );

            modelBuilder.Entity<Account>().HasData(
                new Account { AccountNumber = "2001-CUR", AccountType = AccountType.Current,  CurrentBalance = 15400m,  OpeningDate = new DateTime(2022, 4, 10), BranchCode = "CAI-01" },
                new Account { AccountNumber = "3000-BUS", AccountType = AccountType.Business, CurrentBalance = 120000m, OpeningDate = new DateTime(2021, 8, 1),  BranchCode = "ALX-01" }
            );

            modelBuilder.Entity<CustomerAccount>().HasData(
                new CustomerAccount { CustomerId = 1, AccountNumber = "2001-CUR", OwnershipType = OwnershipType.Primary, OwnershipStartDate = new DateTime(2022, 4, 10), AccountStatus = AccountStatus.Active },
                new CustomerAccount { CustomerId = 2, AccountNumber = "3000-BUS", OwnershipType = OwnershipType.Primary, OwnershipStartDate = new DateTime(2021, 8, 1),  AccountStatus = AccountStatus.Active }
            );

            modelBuilder.Entity<Transaction>().HasData(
                new Transaction { TransactionNumber = 1000, AccountNumber = "2001-CUR", TransactionDate = new DateTime(2022, 4, 10, 10, 0, 0), Amount = 20000m,  TransactionType = TransactionType.Deposit,    Note = "Opening deposit" },
                new Transaction { TransactionNumber = 1001, AccountNumber = "2001-CUR", TransactionDate = new DateTime(2023, 2, 5, 14, 30, 0), Amount = 4600m,   TransactionType = TransactionType.Withdrawal, Note = "ATM withdrawal" },
                new Transaction { TransactionNumber = 1002, AccountNumber = "3000-BUS", TransactionDate = new DateTime(2021, 8, 1, 9, 15, 0),  Amount = 120000m, TransactionType = TransactionType.Deposit,    Note = "Opening deposit" }
            );
        }
    }
}
