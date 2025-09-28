using FakeoverFlow.Backend.Http.Api.Models.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FakeoverFlow.Backend.Http.Api.Models.ModelConfigurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshTokens>
{
    public void Configure(EntityTypeBuilder<RefreshTokens> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.Account)
            .WithMany()
            .HasForeignKey(x => x.UserId);
        
        builder.Property(x => x.RevokedOn)
            .IsRequired(false);
        
        builder.Property(x => x.CreatedOn)
            .IsRequired();
        
        builder.Property(x => x.ExpiresOn)
            .IsRequired()
            .HasDefaultValueSql("NOW() + INTERVAL '10 days'");


        builder.Property(x => x.JTI)
            .IsRequired();

        // Indexes
        
        // For a user, only one refresh token can be created for the same jwt token
        builder.HasIndex(x => new
        {
            NameId = x.JTI,
            x.UserId,
        }).IsUnique();
        
        builder.HasIndex(x => new
        {
            x.Id,
            x.UserId,
            x.ExpiresOn
        });
    }
}