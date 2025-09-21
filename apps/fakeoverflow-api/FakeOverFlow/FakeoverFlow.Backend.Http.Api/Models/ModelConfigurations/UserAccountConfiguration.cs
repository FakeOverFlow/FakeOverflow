using FakeoverFlow.Backend.Http.Api.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FakeoverFlow.Backend.Http.Api.Models.ModelConfigurations;

public class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        builder.HasKey(x => x.Id);

        // Required Properties
        builder.Property(x => x.Email)
            .IsRequired();
        builder.Property(x => x.FirstName)
            .IsRequired();
        builder.Property(x => x.LastName)
            .IsRequired();
        
        // Unique Properties
        builder.HasIndex(x => x.Email)
            .IsUnique();
        builder.HasIndex(x => x.Username)
            .IsUnique();

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder.HasGeneratedTsVectorColumn(
                p => p.VectorText,
                AppConstants.DbVectorTextLanguage,
                p => new
                {
                    p.FirstName,
                    p.LastName,
                    p.Email,
                    p.Username
                })
            .HasIndex(p => p.VectorText)
            .HasMethod("GIN");
        
        // Indexes
        builder.HasIndex(x => x.IsDeleted)
            .HasFilter("\"IsDeleted\" = false")
            .HasDatabaseName("IX_UserAccount_IsDeleted_ActiveOnly");
        builder.HasIndex(x => x.CreatedOn)
            .HasDatabaseName("IX_UserAccount_CreatedOn");

        builder.HasIndex(x => x.UpdatedOn)
            .HasDatabaseName("IX_UserAccount_UpdatedOn");
        
        // Defaults
        builder.Property(x => x.IsDisabled)
            .HasDefaultValue(false);

    }
}