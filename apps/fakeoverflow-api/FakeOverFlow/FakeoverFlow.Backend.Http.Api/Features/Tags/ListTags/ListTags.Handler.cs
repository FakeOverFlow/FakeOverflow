using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FastEndpoints;

namespace FakeoverFlow.Backend.Http.Api.Features.Tags.ListTags;

public partial class ListTags
{
    public class Handler(
        IPostService postService,
        ILogger<ListTags> logger
        ) : Endpoint<ListTags.Request, ListTags.Response>
    {
        public override void Configure()
        {
            Get("/");
            Group<TagGroup>();
            Description(x =>
            {
                x.WithName("ListTags").WithSummary("Lists all tags");
            });
            AllowAnonymous();
        }

        public override async Task<Response> ExecuteAsync(Request req, CancellationToken ct)
        {
            var (tags, tagCounts) = await postService.GetTagsWithCountsAsync(req.Page, req.PageSize, req.SearchTerm, ct);

            return new Response()
            {
                Items = tags.Select(x => new Tags()
                {
                    Id = x.Id,
                    Value = x.Value,
                    PostCount = tagCounts.GetValueOrDefault(x.Id, 0)
                }).ToList()
            };
        }
    } 
}