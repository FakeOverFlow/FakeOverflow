using System.Text;
using FakeoverFlow.Backend.Http.Api.Models.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FakeoverFlow.Backend.Http.Api.Extensions;

public static class SeedingExtension
{
    private static class SeedingConstants
    {
        public static readonly Guid AdminUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public static readonly Guid ModeratorUserId = Guid.Parse("00000000-0000-0000-0000-000000000002");
    }

    public static async Task SeedAsync(this DbContext dbContext, CancellationToken cancellationToken)
    {
        var userAccounts = dbContext.Set<UserAccount>();
        var hasher = dbContext.GetService<IPasswordHasher<UserAccount>>();
        IConfiguration config = dbContext.GetService<IConfiguration>();

        await SeedUserAccountsInternal(
            userAccounts,
            hasher,
            config,
            async (id) => await userAccounts.FindAsync(new object[] { id }, cancellationToken),
            async (account) => await userAccounts.AddAsync(account, cancellationToken)
        );
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public static void Seed(this DbContext dbContext, IConfiguration config)
    {
        var userAccounts = dbContext.Set<UserAccount>();
        var hasher = new PasswordHasher<UserAccount>();

        SeedUserAccountsInternal(
            userAccounts,
            hasher,
            config,
            (id) => Task.FromResult(userAccounts.Find(id)),
            (account) =>
            {
                userAccounts.Add(account);
                return Task.CompletedTask;
            }
        ).GetAwaiter().GetResult();
        
        dbContext.SaveChanges();
    }

    private static async Task SeedUserAccountsInternal(
        DbSet<UserAccount> userAccounts,
        IPasswordHasher<UserAccount> hasher,
        IConfiguration config,
        Func<Guid, Task<UserAccount?>> findFunc,
        Func<UserAccount, Task> addFunc)
    {
        await SeedUserAccount(findFunc, addFunc, userAccounts, hasher, config, 
            SeedingConstants.AdminUserId, "admin@fakeoverflow.github.io", "admin",  "ADMIN", "Account",config["Secrets:AdminPassword"]);

        await SeedUserAccount(findFunc, addFunc, userAccounts, hasher, config, 
            SeedingConstants.ModeratorUserId, "mod@fakeoverflow.github.io", "mod", "MOD", "Account",config["Secrets:ModeratorPassword"]);
    }

    private static async Task SeedUserAccount(
        Func<Guid, Task<UserAccount?>> findFunc,
        Func<UserAccount, Task> addFunc,
        DbSet<UserAccount> userAccounts,
        IPasswordHasher<UserAccount> hasher,
        IConfiguration config,
        Guid userId,
        string email,
        string username,
        string firstName,
        string lastName,
        string? password)
    {
        var existing = await findFunc(userId);
        if (existing != null)
            return;
        
        Console.WriteLine($"Seeding {username}");

        if (string.IsNullOrWhiteSpace(password))
            throw new Exception($"{username} password not configured");

        var hashedPassword = hasher.HashPassword(null, password);
        var bytes = Encoding.UTF8.GetBytes(hashedPassword);

        var user = new UserAccount()
        {
            Id = userId,
            Email = email,
            Username = username,
            FirstName = firstName,
            LastName = lastName,
            Password = bytes,
            VerifiedOn = DateTimeOffset.UtcNow,
            CreatedOn = DateTimeOffset.UtcNow,
            UpdatedOn = DateTimeOffset.UtcNow,
            CreatedBy = userId,
            UpdatedBy = userId,
        };

        await addFunc(user);
    }
}
