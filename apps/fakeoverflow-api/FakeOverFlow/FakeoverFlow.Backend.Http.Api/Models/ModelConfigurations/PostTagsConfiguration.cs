using FakeoverFlow.Backend.Http.Api.Models.Posts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FakeoverFlow.Backend.Http.Api.Models.ModelConfigurations;

public class PostTagsConfiguration : IEntityTypeConfiguration<PostTags>
{
    public void Configure(EntityTypeBuilder<PostTags> builder)
    {
        builder
            .HasKey(pt => new { pt.PostId, pt.TagId });

        builder
            .HasOne(pt => pt.Posts)
            .WithMany(p => p.Tags)
            .HasForeignKey(pt => pt.PostId);

        builder
            .HasOne(pt => pt.Tag)
            .WithMany(t => t.PostTags)
            .HasForeignKey(pt => pt.TagId);
    }
}