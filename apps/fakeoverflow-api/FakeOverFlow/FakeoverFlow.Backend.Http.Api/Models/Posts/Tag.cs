using FakeoverFlow.Backend.Abstraction.Models;
using FakeoverFlow.Backend.Http.Api.Utils;
using NpgsqlTypes;

namespace FakeoverFlow.Backend.Http.Api.Models.Posts;

public class Tag : IIntEntity
{
    public int Id { get; set; }
    
    public string Value { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public string Color { get; set; } = TagColorPalette.GetRandomColor();
    public ICollection<PostTags> PostTags { get; set; } = new List<PostTags>();
    
    public NpgsqlTsVector VectorText { get; set; } = null!;

}