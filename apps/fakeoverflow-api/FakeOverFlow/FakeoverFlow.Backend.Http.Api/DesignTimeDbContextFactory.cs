using FakeoverFlow.Backend.Http.Api.Extensions;
using FakeoverFlow.Backend.Http.Api.Models.Accounts;

namespace FakeoverFlow.Backend.Http.Api;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var services = new ServiceCollection();

        // Register configuration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Register password hasher
        services.AddScoped<IPasswordHasher<UserAccount>, PasswordHasher<UserAccount>>();
        services.ConfigureContext();
        
        // Register DbContext
        services.ConfigureDatabase(configuration);

        var serviceProvider = services.BuildServiceProvider();

        return serviceProvider.GetRequiredService<AppDbContext>();
    }
}
