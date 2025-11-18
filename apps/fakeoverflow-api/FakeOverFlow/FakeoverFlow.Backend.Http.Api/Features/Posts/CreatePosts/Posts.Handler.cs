using FakeoverFlow.Backend.Abstraction.Context;
using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts.CreatePosts;

public static partial class Posts
{
    public class Handler(
        IPostService postService,
        IContextFactory contextFactory,
        ILogger<Handler> logger
    ) : Endpoint<Request, Results<Ok<Response>, BadRequest<ErrorResponse>, ProblemHttpResult>>
    {
        public override void Configure()
        {
            Description(x => x.WithName("CreatePost"));
            Post("");
            Group<PostGroup>();
        }

        public override async Task<Results<Ok<Response>, BadRequest<ErrorResponse>, ProblemHttpResult>> ExecuteAsync(
            Request req,
            CancellationToken ct)
        {
            var post = await postService.CreatePostAsync(req, ct);


            return TypedResults.Ok(new Response()
            {
                Id = post.Id,
            });
        }
    }
}