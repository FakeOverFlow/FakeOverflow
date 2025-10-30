using FakeoverFlow.Backend.Http.Api.Constants;
using FakeoverFlow.Backend.Http.Api.Models.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FakeoverFlow.Backend.Http.Api.Models.ModelConfigurations;

public class PostConfiguration : IEntityTypeConfiguration<Posts.Posts>
{
    public void Configure(EntityTypeBuilder<Posts.Posts> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Title)
            .IsRequired();
        
        builder.Property(x => x.CreatedBy)
            .IsRequired();
        
        builder.HasOne<UserAccount>(x => x.CreatedByAccount)
            .WithMany()
            .HasForeignKey(p => p.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(p => p.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasGeneratedTsVectorColumn(
                p => p.VectorText,
                AppConstants.DbVectorTextLanguage,
                p => new
                {
                    p.Title
                })
            .HasIndex(p => p.VectorText)
            .HasMethod("GIN");
    }
}