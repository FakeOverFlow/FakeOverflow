using FakeoverFlow.Backend.Abstraction.Context;
using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FakeoverFlow.Backend.Http.Api.Extensions;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts.ListPosts;

public static partial class ListPosts
{
    public class Handler : EndpointWithoutRequest<Results<Ok<Response>, ErrorResponse, ProblemDetails>>
    {
        private readonly IContextFactory _contextFactory;
        private readonly ILogger<Handler> _logger;
        private readonly IPostService _postService;

        public Handler(
            ILogger<Handler> logger,
            IContextFactory contextFactory,
            IPostService postService)
        {
            _logger = logger;
            _contextFactory = contextFactory;
            _postService = postService;
        }

        public override void Configure()
        {
            Get("/");
            Group<PostGroup>();
            Description(x => x.WithName("ListPosts"));
            AllowAnonymous();
        }

        public override async Task<Results<Ok<Response>, ErrorResponse, ProblemDetails>> ExecuteAsync(
            CancellationToken ct)
        {
            var q = HttpContext.Request.Query;

            int page = 1;
            int pageSize = 20;

            var tags = new List<string>();
            if (q.TryGetValue("tags", out var tagValues))
            {
                tags = tagValues
                    .SelectMany(v => v.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    .Select(t => t.Trim().ToLower())
                    .Distinct()
                    .ToList();
            }

            if (q.TryGetValue("page", out var pv) && int.TryParse(pv.FirstOrDefault(), out var p))
                page = p;

            if (q.TryGetValue("pageSize", out var sv) && int.TryParse(sv.FirstOrDefault(), out var s))
                pageSize = s;

            if (page < 1)
                AddError(new ValidationFailure("page", "page must be >= 1"));

            if (pageSize < 1 || pageSize > 200)
                AddError(new ValidationFailure("pageSize", "pageSize must be between 1 and 200"));

            ThrowIfAnyErrors();

            var listResult = await _postService.ListPostsAsync(page, pageSize, tags, ct);

            if (listResult.IsFailure)
                return listResult.Error!.ToFastEndpointError();

            var (items, totalCount) = listResult.Value!;

            var response = new Response
            {
                Posts = items.Select(item =>
                {
                    var post = item.post;
                    var content = item.content;

                    return new PostSummary
                    {
                        PostId = post.Id ?? string.Empty,
                        Title = post.Title ?? string.Empty,

                        Content = content?.Content ?? string.Empty,
                        //Tags = post.Tags,
                        Views = post.Views,
                        //Votes = post.Votes
                        CreatedOn = post.CreatedOn,
                        UserId = post.CreatedByAccount.Id,
                        UserName = post.CreatedByAccount.Username
                    };
                }).ToList(),

                TotalCount = (int)totalCount,
                Page = page,
                PageSize = pageSize
            };

            return TypedResults.Ok(response);
        }
    }
}