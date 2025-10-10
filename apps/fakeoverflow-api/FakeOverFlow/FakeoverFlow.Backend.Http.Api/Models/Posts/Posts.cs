using FakeoverFlow.Backend.Abstraction.Models;
using FakeoverFlow.Backend.Http.Api.Models.Accounts;
using NpgsqlTypes;

namespace FakeoverFlow.Backend.Http.Api.Models.Posts;

public class Posts : IStringEntity, IPostAuditableEntity, IPutAuditableEntity
{
    public string Id { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public UserAccount CreatedByAccount { get; set; } = null!;
    public Guid UpdatedBy { get; set; }
    public DateTimeOffset UpdatedOn { get; set; }
    public UserAccount UpdatedByAccount { get; set; } = null!;
    public string Title { get; set; } = null!;
    public long Views { get; set; }
    public long Votes { get; set; } 
    public NpgsqlTsVector VectorText { get; set; } = null!;
}