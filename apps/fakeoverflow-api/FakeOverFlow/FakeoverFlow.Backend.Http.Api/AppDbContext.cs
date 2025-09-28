using FakeoverFlow.Backend.Abstraction.Context;
using FakeoverFlow.Backend.Abstraction.Models;
using FakeoverFlow.Backend.Http.Api.Models;
using FakeoverFlow.Backend.Http.Api.Models.Accounts;
using Microsoft.EntityFrameworkCore;

namespace FakeoverFlow.Backend.Http.Api;

public class AppDbContext : DbContext
{
    
    private readonly IContextFactory _contextFactory;

    // Tables
    public DbSet<UserAccount> UserAccounts { get; set; } = null!;
    public DbSet<UserAccountVerification> UserAccountVerifications { get; set; } = null!;
    
    public DbSet<RefreshTokens> RefreshTokens { get; set; } = null!;

    #region Internals

    protected AppDbContext(IContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public AppDbContext(DbContextOptions options, IContextFactory contextFactory) : base(options)
    {
        _contextFactory = contextFactory;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override int SaveChanges()
    {
        ApplyAudit();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await ApplyAudit();
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    private Task ApplyAudit()
    {
        var entries = ChangeTracker.Entries();
        var requestContext = _contextFactory.RequestContext;
        if (!requestContext.IsAuthenticated) return Task.CompletedTask;
        
        foreach (var entry in entries)
        {
            if (entry.Entity is IPostAuditableEntity createdEntity && entry.State == EntityState.Added)
            {
                createdEntity.AuditCreation(requestContext.UserId, DateTimeOffset.UtcNow);
            }

            if (entry.Entity is IPutAuditableEntity updatedEntity && entry.State == EntityState.Modified)
            {
                updatedEntity.AuditModification(requestContext.UserId, DateTimeOffset.UtcNow);
            }

            if (entry.Entity is ISoftDeleteEntity softDeleteEntity && entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                softDeleteEntity.IsDeleted = true;

                if (softDeleteEntity is IPutAuditableEntity deletedAuditEntity)
                {
                    deletedAuditEntity.AuditModification(requestContext.UserId, DateTimeOffset.UtcNow);
                }
            }
        }

        return Task.CompletedTask;
    }

    #endregion
}