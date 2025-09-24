using FakeoverFlow.Backend.Abstraction.Models;
using FakeoverFlow.Backend.Http.Api.Models.Enums;
using NpgsqlTypes;

namespace FakeoverFlow.Backend.Http.Api.Models.Accounts;

public class UserAccount : IEntity, IPutAuditableEntity, ISoftDeleteEntity, IVectorSearchableEntity
{
    public Guid Id { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public Guid UpdatedBy { get; set; }
    public DateTimeOffset UpdatedOn { get; set; }
    public bool IsDeleted { get; set; }
    public NpgsqlTsVector VectorText { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } =  null!;
    public string Email { get; set; } = null!;
    public string Username { get; set; } = null!;
    public byte[]? Password { get; set; } = null;
    public UserRoles Role { get; set; } = UserRoles.User;
    public DateTimeOffset? VerifiedOn { get; set; } = null;
    public string? ProfileImageUrl { get; set; } = null;
    public bool IsDisabled { get; set; } = false;
    
    public UserAccountSettings Settings { get; set; } = new();

}