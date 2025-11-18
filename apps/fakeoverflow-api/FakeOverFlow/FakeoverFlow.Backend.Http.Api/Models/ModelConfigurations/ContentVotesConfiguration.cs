using FakeoverFlow.Backend.Http.Api.Models.Posts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FakeoverFlow.Backend.Http.Api.Models.ModelConfigurations;

public class ContentVotesConfiguration : IEntityTypeConfiguration<ContentVotes>
{
    public void Configure(EntityTypeBuilder<ContentVotes> builder)
    {
        builder.HasKey(cv => new { cv.UserId, cv.ContentId });

        builder.HasOne(cv => cv.User)
            .WithMany()
            .HasForeignKey(cv => cv.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cv => cv.PostContent)
            .WithMany(pc => pc.VotesBy)
            .HasForeignKey(cv => cv.ContentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(cv => cv.UpVote)
            .IsRequired();
    }
}