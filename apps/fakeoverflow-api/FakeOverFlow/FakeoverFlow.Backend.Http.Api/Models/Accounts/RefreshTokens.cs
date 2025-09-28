using FakeoverFlow.Backend.Abstraction.Models;

namespace FakeoverFlow.Backend.Http.Api.Models.Accounts;

public class RefreshTokens : IStringEntity
{
    public required string Id { get; set; }
    
    public required string JTI { get; set; }
    
    public Guid UserId { get; set; }
    
    public UserAccount Account { get; set; } = null!;
    
    public DateTimeOffset CreatedOn { get; set; }
    
    public DateTimeOffset ExpiresOn { get; set; }
    
    public DateTimeOffset? RevokedOn { get; set; } = null;
}