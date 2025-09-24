using FakeoverFlow.Backend.Http.Api.Models.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FakeoverFlow.Backend.Http.Api.Models.ModelConfigurations;

public class UserAccountVerificationConfiguration : IEntityTypeConfiguration<UserAccountVerification>
{
    public void Configure(EntityTypeBuilder<UserAccountVerification> builder)
    {
        builder.HasKey(x => x.VerificationToken);

        builder.HasOne(x => x.Account)
            .WithOne()
            .HasForeignKey<UserAccountVerification>(x => x.UserId);
    }
}