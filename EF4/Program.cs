using EF4.Data;
using EF4.Operations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EF4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var context = new AppDbContext();
            var operations = new BankOperations(context);

            bool running = true;
            while (running)
            {
                ConsoleHelper.ClearScreen();
                PrintMenu();

                Console.Write("  Enter choice: ");
                string input = (Console.ReadLine() ?? string.Empty).Trim();
                Console.WriteLine();

                if (!int.TryParse(input, out int choice) || choice < 0 || choice > 5)
                {
                    ConsoleHelper.Error("  Invalid choice. Please enter a number from 0 to 5.");
                    ConsoleHelper.Pause();
                    continue;
                }

                if (choice == 0)
                {
                    running = false;
                    break;
                }

                try
                {
                    switch (choice)
                    {
                        case 1: operations.AddCustomer();               break;
                        case 2: operations.OpenAccount();               break;
                        case 3: operations.UpdateAccountStatus();       break;
                        case 4: operations.RemoveAccountFromCustomer(); break;
                        case 5: operations.ListCustomers();             break;
                    }
                }
                catch (DbUpdateException ex)
                {
                    ConsoleHelper.Error($"Could not save changes: {ex.InnerException?.Message ?? ex.Message}");
                    context.ChangeTracker.Clear();
                }
                catch (SqlException ex)
                {
                    ConsoleHelper.Error($"Database error: {ex.Message}");
                    ConsoleHelper.Error("Make sure SQL Server is running and you ran 'Update-Database'.");
                }

                ConsoleHelper.Pause();
            }

            Console.WriteLine("Goodbye!");
        }

        private static void PrintMenu()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("     National Bank - Management");
            Console.WriteLine("========================================");
            Console.WriteLine("  1) Add a new Customer");
            Console.WriteLine("  2) Open a new Account for a Customer");
            Console.WriteLine("  3) Update Account Status (Active / Closed)");
            Console.WriteLine("  4) Remove an Account from a Customer");
            Console.WriteLine("  5) List all Customers (with accounts)");
            Console.WriteLine("  0) Exit");
            Console.WriteLine("----------------------------------------");
        }
    }
}
