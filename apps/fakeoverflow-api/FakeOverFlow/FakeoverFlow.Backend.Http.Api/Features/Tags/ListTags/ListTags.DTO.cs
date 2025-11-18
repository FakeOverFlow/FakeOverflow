namespace FakeoverFlow.Backend.Http.Api.Features.Tags.ListTags;

public partial class ListTags
{
    public class Response
    {
        public List<Tags> Items { get; set; } = [];
    }

    public class Request
    {
        public string? SearchTerm { get; set; } = null;
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 100;
    }

    public class Tags
    {
        public required string Value { get; set; }
        
        public int Id { get; set; }
        
        public long PostCount { get; set; }
    }
}