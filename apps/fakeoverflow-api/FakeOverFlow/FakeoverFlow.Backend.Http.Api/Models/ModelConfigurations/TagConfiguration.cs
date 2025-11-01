using FakeoverFlow.Backend.Http.Api.Constants;
using FakeoverFlow.Backend.Http.Api.Models.Posts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FakeoverFlow.Backend.Http.Api.Models.ModelConfigurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.Value)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.HasIndex(x => x.Value)
            .IsUnique();
        
        builder.Property(t => t.Description)
            .HasMaxLength(200)
            .HasDefaultValue("");
        
        builder.Property(t => t.Color)
            .HasMaxLength(20)
            .IsRequired();
        
        builder.HasMany(t => t.PostTags)
            .WithOne(pt => pt.Tag)
            .HasForeignKey(pt => pt.TagId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasGeneratedTsVectorColumn(
                p => p.VectorText,
                AppConstants.DbVectorTextLanguage,
                p => new
                {
                    p.Value,
                    p.Description
                })
            .HasIndex(p => p.VectorText)
            .HasMethod("GIN");
    }
}