namespace FakeoverFlow.Backend.Abstraction.Context;

public class NonValidateRequestContext : IRequestContext
{
    public bool IsAuthenticated { get; } = false;
    public Guid UserId { get; } = Guid.Empty;
    public string Role { get; } = string.Empty;
    public bool IsAdmin { get; } = false;
    public string Email { get; } = string.Empty;
    public string UserName { get; } = string.Empty;
    public string FirstName { get; } = string.Empty;
    public string LastName { get; } = string.Empty;
    public string TokenId { get; } = string.Empty;
}