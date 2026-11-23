using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;
using EF4.Data;
using EF4.Models;
using Microsoft.EntityFrameworkCore;

namespace EF4.Operations
{
    public class BankOperations
    {
        private static readonly CultureInfo Num = CultureInfo.InvariantCulture;

        private readonly AppDbContext _context;

        public BankOperations(AppDbContext context)
        {
            _context = context;
        }

        public void AddCustomer()
        {
            Console.WriteLine("--- Add New Customer ---");

            string fullName = ConsoleHelper.ReadString("Full Name      : ", ValidateFullName);

            string nationalId = ConsoleHelper.ReadString("National ID    : ", value =>
            {
                if (!Regex.IsMatch(value, @"^\d{10,14}$"))
                    return "National ID must be 10 to 14 digits.";
                if (_context.Customers.Any(c => c.NationalId == value))
                    return "A customer with this National ID already exists.";
                return null;
            });

            DateTime dateOfBirth = ConsoleHelper.ReadDate("Date of Birth  : (yyyy-MM-dd) ", value =>
            {
                if (value.Date > DateTime.Today) return "Date of birth cannot be in the future.";
                if (value.Year < 1900)           return "Date of birth is too far in the past.";
                return null;
            });

            string email   = ConsoleHelper.ReadString("Email          : ", ValidateEmail);
            string phone   = ConsoleHelper.ReadString("Phone          : ", ValidatePhone);
            string address = ConsoleHelper.ReadString("Address        : ", value =>
                value.Length > 200 ? "Address must be 200 characters or fewer." : null);

            CustomerType customerType = ConsoleHelper.ReadEnumChoice<CustomerType>("Customer Type:");

            var customer = new Customer
            {
                FullName = fullName,
                NationalId = nationalId,
                DateOfBirth = dateOfBirth,
                Email = email,
                PhoneNumber = phone,
                Address = address,
                CustomerType = customerType
            };

            _context.Customers.Add(customer);
            _context.SaveChanges();

            Console.WriteLine();
            ConsoleHelper.Success($"Customer created successfully. CustomerId = {customer.Id}");
        }

        public void OpenAccount()
        {
            Console.WriteLine("--- Open New Account ---");

            string accountNumber = ConsoleHelper.ReadString("Account Number : ", ValidateAccountNumber);
            AccountType accountType = ConsoleHelper.ReadEnumChoice<AccountType>("Account Type:");
            string branchCode = ConsoleHelper.ReadString("Branch Code    : ").ToUpperInvariant();
            int customerId = ConsoleHelper.ReadInt("Customer Id    : ", 1);
            OwnershipType role = ConsoleHelper.ReadEnumChoice<OwnershipType>("Ownership Role:");

            ConsoleHelper.Info($"Validating branch '{branchCode}' and customer #{customerId}...");

            if (!_context.Branches.Any(b => b.Code == branchCode))
            {
                ConsoleHelper.Error($"Branch '{branchCode}' does not exist.");
                return;
            }

            if (!_context.Customers.Any(c => c.Id == customerId))
            {
                ConsoleHelper.Error($"Customer #{customerId} does not exist.");
                return;
            }

            Account? existing = _context.Accounts
                                        .Include(a => a.CustomerAccounts)
                                        .FirstOrDefault(a => a.AccountNumber == accountNumber);

            if (existing is null)
            {
                if (role != OwnershipType.Primary)
                {
                    ConsoleHelper.Error("A new account must be opened with a Primary owner. " +
                                        "Add CoHolders after the account exists.");
                    return;
                }

                var account = new Account
                {
                    AccountNumber = accountNumber,
                    AccountType = accountType,
                    CurrentBalance = 0m,
                    OpeningDate = DateTime.Today,
                    BranchCode = branchCode
                };

                account.CustomerAccounts.Add(new CustomerAccount
                {
                    CustomerId = customerId,
                    OwnershipType = role,
                    OwnershipStartDate = DateTime.Today,
                    AccountStatus = AccountStatus.Active
                });

                _context.Accounts.Add(account);
                _context.SaveChanges();

                ConsoleHelper.Success($"Account '{accountNumber}' created and linked to customer {customerId} as {role} owner.");
                return;
            }

            if (existing.BranchCode != branchCode)
            {
                ConsoleHelper.Error($"Account '{accountNumber}' already exists at branch '{existing.BranchCode}'.");
                return;
            }

            if (existing.AccountType != accountType)
            {
                ConsoleHelper.Error($"Account '{accountNumber}' already exists as a {existing.AccountType} account.");
                return;
            }

            if (existing.CustomerAccounts.Any(ca => ca.CustomerId == customerId))
            {
                ConsoleHelper.Error($"Customer {customerId} is already linked to account '{accountNumber}'.");
                return;
            }

            if (role == OwnershipType.Primary &&
                existing.CustomerAccounts.Any(ca => ca.OwnershipType == OwnershipType.Primary))
            {
                ConsoleHelper.Error($"Account '{accountNumber}' already has a Primary owner. Choose CoHolder instead.");
                return;
            }

            _context.CustomerAccounts.Add(new CustomerAccount
            {
                CustomerId = customerId,
                AccountNumber = accountNumber,
                OwnershipType = role,
                OwnershipStartDate = DateTime.Today,
                AccountStatus = AccountStatus.Active
            });
            _context.SaveChanges();

            ConsoleHelper.Success($"Customer {customerId} linked to existing account '{accountNumber}' as {role}.");
        }

        public void UpdateAccountStatus()
        {
            Console.WriteLine("--- Update Account Status ---");

            string accountNumber = ConsoleHelper.ReadString("Account Number : ");
            int customerId = ConsoleHelper.ReadInt("Customer Id    : ", 1);

            CustomerAccount? link = _context.CustomerAccounts
                .FirstOrDefault(ca => ca.AccountNumber == accountNumber && ca.CustomerId == customerId);

            if (link is null)
            {
                ConsoleHelper.Error($"Customer {customerId} is not linked to account '{accountNumber}'.");
                return;
            }

            Console.WriteLine($"Current Status : {link.AccountStatus}");
            AccountStatus newStatus = ConsoleHelper.ReadEnumChoice<AccountStatus>("New Status:");

            if (newStatus == link.AccountStatus)
            {
                ConsoleHelper.Info($"Status is already {newStatus}. Nothing changed.");
                return;
            }

            link.AccountStatus = newStatus;
            _context.SaveChanges();

            ConsoleHelper.Success($"Status updated to {newStatus}.");
        }

        public void RemoveAccountFromCustomer()
        {
            Console.WriteLine("--- Remove Account From Customer ---");

            string accountNumber = ConsoleHelper.ReadString("Account Number : ");
            int customerId = ConsoleHelper.ReadInt("Customer Id    : ", 1);
            Console.WriteLine();

            CustomerAccount? link = _context.CustomerAccounts
                .FirstOrDefault(ca => ca.AccountNumber == accountNumber && ca.CustomerId == customerId);

            if (link is null)
            {
                ConsoleHelper.Error($"  Customer {customerId} is not linked to account '{accountNumber}'.");
                return;
            }

            List<CustomerAccount> otherOwners = _context.CustomerAccounts
                .Where(ca => ca.AccountNumber == accountNumber && ca.CustomerId != customerId)
                .OrderBy(ca => ca.OwnershipStartDate)
                .ToList();

            _context.CustomerAccounts.Remove(link);

            bool accountRemoved = false;
            CustomerAccount? promoted = null;

            if (otherOwners.Count == 0)
            {
                // Last owner gone -> the account (and its transactions, via cascade) goes too
                Account account = _context.Accounts.First(a => a.AccountNumber == accountNumber);
                _context.Accounts.Remove(account);
                accountRemoved = true;
            }
            else if (link.OwnershipType == OwnershipType.Primary &&
                     !otherOwners.Any(o => o.OwnershipType == OwnershipType.Primary))
            {
                // Keep exactly one Primary: promote the longest-standing CoHolder
                promoted = otherOwners[0];
                promoted.OwnershipType = OwnershipType.Primary;
            }

            _context.SaveChanges();

            ConsoleHelper.Success("  Ownership link deleted.");
            if (accountRemoved)
                ConsoleHelper.Info($"      That was the last owner - account '{accountNumber}' was also removed.");
            if (promoted is not null)
                ConsoleHelper.Info($"      Customer {promoted.CustomerId} is now the Primary owner of '{accountNumber}'.");
        }

        public void ListCustomers()
        {
            Console.WriteLine("--- All Customers ---");
            Console.WriteLine();

            List<Customer> customers = _context.Customers
                .AsNoTracking()
                .Include(c => c.CustomerAccounts.OrderBy(ca => ca.AccountNumber))
                    .ThenInclude(ca => ca.Account)
                        .ThenInclude(a => a.Branch)
                .OrderBy(c => c.Id)
                .ToList();

            if (customers.Count == 0)
            {
                Console.WriteLine("  (no customers)");
                return;
            }

            foreach (Customer customer in customers)
            {
                Console.WriteLine($"  #{customer.Id} {customer.FullName} ({customer.CustomerType})");

                if (customer.CustomerAccounts.Count == 0)
                {
                    Console.WriteLine("      (no accounts)");
                    continue;
                }

                foreach (CustomerAccount ca in customer.CustomerAccounts)
                {
                    Account a = ca.Account;
                    Console.WriteLine(
                        $"      {a.AccountNumber,-10} {a.AccountType,-9} Balance: {a.CurrentBalance.ToString("N2", Num),13}   " +
                        $"{ca.OwnershipType,-9} {ca.AccountStatus,-7} @ {a.Branch.Name}");
                }
            }
        }

        private static string? ValidateFullName(string value)
        {
            if (value.Length < 3 || value.Length > 100)
                return "Full name must be between 3 and 100 characters.";
            if (!Regex.IsMatch(value, @"^[\p{L}][\p{L}\s.'&-]*$"))
                return "Full name can contain letters, spaces and . ' & - only.";
            return null;
        }

        private static string? ValidateEmail(string value)
        {
            if (value.Length > 100)
                return "Email must be 100 characters or fewer.";
            try
            {
                var address = new MailAddress(value);
                if (address.Address != value || !address.Host.Contains('.'))
                    return "Invalid email address.";
                return null;
            }
            catch (FormatException)
            {
                return "Invalid email address.";
            }
        }

        private static string? ValidatePhone(string value) =>
            Regex.IsMatch(value, @"^\+?\d{8,15}$")
                ? null
                : "Phone must be 8 to 15 digits (optionally starting with +).";

        private static string? ValidateAccountNumber(string value) =>
            Regex.IsMatch(value, @"^[A-Za-z0-9-]{3,20}$")
                ? null
                : "Account number must be 3 to 20 characters (letters, digits or '-').";
    }
}
