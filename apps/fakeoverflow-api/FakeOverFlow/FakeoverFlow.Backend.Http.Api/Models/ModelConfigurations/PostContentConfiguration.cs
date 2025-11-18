using FakeoverFlow.Backend.Http.Api.Constants;
using FakeoverFlow.Backend.Http.Api.Models.Posts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FakeoverFlow.Backend.Http.Api.Models.ModelConfigurations;

public class PostContentConfiguration : IEntityTypeConfiguration<PostContent>
{
    public void Configure(EntityTypeBuilder<PostContent> builder)
    {
        builder.HasKey(pc => pc.Id);

        builder.HasOne(pc => pc.CreatedByAccount)
            .WithMany()
            .HasForeignKey(pc => pc.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pc => pc.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(pc => pc.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pc => pc.Post)
            .WithMany(p => p.Contents)
            .HasForeignKey(pc => pc.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pc => pc.Post)
            .WithMany()
            .HasForeignKey(pc => pc.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(pc => pc.Content)
            .IsRequired();

        builder.HasIndex(x => new { x.PostId, x.ContentType })
            .HasFilter("\"ContentType\" = 'questions'")
            .IsUnique();

        builder.HasGeneratedTsVectorColumn(
                p => p.VectorText,
                AppConstants.DbVectorTextLanguage,
                p => new
                {
                    p.Content
                })
            .HasIndex(p => p.VectorText)
            .HasMethod("GIN");

        builder.HasMany(pc => pc.VotesBy)
            .WithOne(v => v.PostContent)
            .HasForeignKey(v => v.ContentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}