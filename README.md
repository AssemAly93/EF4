# EF4 - National Bank Management System

EF Core 8 Code-First console app (SQL Server), Session 04 assignment.

## Run it

1. Open `EF4.sln` in Visual Studio (packages restore automatically).
2. Check the connection string in `EF4/Data/AppDbContext.cs`
   (default: `Server=.;Database=NationalBankDb;Trusted_Connection=True;TrustServerCertificate=True;`).
3. The `InitialCreate` migration (tables + seed data) is already in `EF4/Migrations`.
   Create the database from the Package Manager Console (Default project = EF4):

   ```
   Update-Database
   ```

   CLI equivalent: `dotnet ef database update`

4. Run the project (F5 / Ctrl+F5).

To keep the seed data in a separate migration, delete the `Migrations` folder, comment out
`modelBuilder.Seed();` in `AppDbContext.OnModelCreating`, run `Add-Migration InitialCreate`, uncomment it, then run
`Add-Migration SeedData` and `Update-Database`.

## Structure

- `Models/`: Branch, Manager, Customer, Account, CustomerAccount (the M:M "Have" join entity), Transaction, enums
- `Data/Configurations/`: Fluent API configuration, one class per entity
- `Data/Seeding/DataSeeder.cs`: HasData seed (Branches, Managers, plus sample customers, accounts and transactions)
- `Operations/`: menu actions (`BankOperations`) and input/validation helpers (`ConsoleHelper`)
- `Program.cs`: menu loop

## Relationships (from the ER diagram)

| Relationship | Type | Configured as |
|---|---|---|
| Branch - Manager (Manage) | 1 : 1 | FK `Manager.BranchCode` (unique), Restrict |
| Branch - Account (Belong) | 1 : M | FK `Account.BranchCode` (required), Restrict |
| Account - Customer (Have) | M : M | `CustomerAccount` with composite PK (CustomerId, AccountNumber) and the attributes OwnershipType, OwnershipStartDate, AccountStatus. Cascade from both sides |
| Account - Transaction (Related) | 1 : M | FK `Transaction.AccountNumber` (required), Cascade |

Enums are stored as strings.

## Business rules applied

- A new account must be opened with a **Primary** owner. Entering an existing account number
  adds the customer as an extra owner (joint account). That account must be at the same branch
  and of the same type, and it can have only one Primary owner.
- Option 4: if the removed owner was the last one, the account (and its transactions) is deleted.
  If the Primary owner leaves and CoHolders remain, the longest-standing CoHolder becomes Primary.
