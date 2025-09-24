using FakeoverFlow.Backend.Abstraction.Models;

namespace FakeoverFlow.Backend.Http.Api.Models.Accounts;

public class UserAccountVerification
{
    public string VerificationToken { get; set; } = null!;
    public Guid UserId { get; set; }
    public UserAccount Account { get; set; } = null!;
    public DateTimeOffset CreatedOn { get; set; }
}