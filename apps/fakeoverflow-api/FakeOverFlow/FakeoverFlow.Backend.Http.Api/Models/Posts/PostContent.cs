using FakeoverFlow.Backend.Abstraction.Models;
using FakeoverFlow.Backend.Http.Api.Models.Accounts;
using FakeoverFlow.Backend.Http.Api.Models.Enums;
using NpgsqlTypes;

namespace FakeoverFlow.Backend.Http.Api.Models.Posts;

public class PostContent : IGuidEntity, IPostAuditableEntity, IPutAuditableEntity
{
    public UserAccount CreatedByAccount { get; set; } = null!;
    public UserAccount UpdatedByAccount { get; set; } = null!;
    public string Content { get; set; } = null!;
    public ContentType ContentType { get; set; }
    public string PostId { get; set; } = null!;
    public Posts Post { get; set; } = null!;
    public NpgsqlTsVector VectorText { get; set; } = null!;

    public long Votes { get; set; } = 0;

    public ICollection<ContentVotes> VotesBy { get; set; } = new List<ContentVotes>();
    public Guid Id { get; set; }
    public Guid CreatedBy { get; set; }

    public DateTimeOffset CreatedOn { get; set; }
    public Guid UpdatedBy { get; set; }
    public DateTimeOffset UpdatedOn { get; set; }
}