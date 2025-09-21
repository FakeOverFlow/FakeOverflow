using System.Security.Claims;
using FakeoverFlow.Backend.Abstraction.Context;
using FakeoverFlow.Backend.Http.Api.Models.Enums;

namespace FakeoverFlow.Backend.Http.Api.Context;

public class RequestContext : IRequestContext
{

    public RequestContext(ClaimsPrincipal user)
    {
        var nameid = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(nameid, out var guid)) 
            UserId = guid;    
        else UserId = Guid.Empty;

        Role = user.FindFirstValue(ClaimTypes.Role) ?? nameof(UserRoles.User);
        IsAdmin = Role == nameof(UserRoles.Admin);
        Email = user.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        UserName = user.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        FirstName = user.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;
        FirstName = user.FindFirstValue(ClaimTypes.Surname) ?? string.Empty;
        TokenId = user.FindFirstValue(ClaimTypes.Sid) ?? string.Empty;
    }
    
    public bool IsAuthenticated { get; } = true;
    public Guid UserId { get; }
    public string Role { get; }
    public bool IsAdmin { get; }
    public string Email { get; }
    public string UserName { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string TokenId { get; }
}