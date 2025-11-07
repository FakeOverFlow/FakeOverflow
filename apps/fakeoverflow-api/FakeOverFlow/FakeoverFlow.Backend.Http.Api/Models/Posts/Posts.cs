using FakeoverFlow.Backend.Abstraction.Models;
using FakeoverFlow.Backend.Http.Api.Models.Accounts;
using NpgsqlTypes;

namespace FakeoverFlow.Backend.Http.Api.Models.Posts;

public class Posts : IStringEntity, IPostAuditableEntity, IPutAuditableEntity
{
    public UserAccount CreatedByAccount { get; set; } = null!;
    public UserAccount UpdatedByAccount { get; set; } = null!;
    public string Title { get; set; } = null!;
    public long Views { get; set; }
    public NpgsqlTsVector VectorText { get; set; } = null!;

    public ICollection<PostContent> Contents { get; set; } = new List<PostContent>();

    public ICollection<PostTags> Tags { get; set; } = new List<PostTags>();
    public Guid CreatedBy { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public Guid UpdatedBy { get; set; }
    public DateTimeOffset UpdatedOn { get; set; }
    public string Id { get; set; }
}