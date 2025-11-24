using FakeoverFlow.Backend.Abstraction.Context;
using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FakeoverFlow.Backend.Http.Api.Extensions;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts.ListPosts;

public static partial class ListPosts
{
    public class Handler : Endpoint<Request, Results<Ok<Response>, ErrorResponse, ProblemDetails>>
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
            Request request,
            CancellationToken ct)
        {
            var q = HttpContext.Request.Query;

            int page = request.Page ?? 1;
            int pageSize = request.PageSize ?? 30;

            var tags = (request.Tag ?? string.Empty)
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim().ToLower())
                .Distinct()
                .ToList();

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
                        Tags = post.Tags.Select(x => x.Tag.Value).ToList(),
                        Views = post.Views,
                        Votes = content?.Votes ?? 0,
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