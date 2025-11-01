using FakeoverFlow.Backend.Abstraction.Models;

namespace FakeoverFlow.Backend.Http.Api.Models.Posts;

public class PostTags
{
    public int TagId { get; set; }

    public Tag Tag { get; set; } = null!;

    public string PostId { get; set; } = null!;

    public Posts Posts { get; set; } = null!;
}