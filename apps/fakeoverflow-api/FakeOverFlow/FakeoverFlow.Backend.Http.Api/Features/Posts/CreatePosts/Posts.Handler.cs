using FakeoverFlow.Backend.Abstraction.Context;
using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FakeoverFlow.Backend.Http.Api.Events.Models;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts.CreatePosts;

public static partial class Posts
{
    public class Handler(
        IPostService postService,
        IContextFactory contextFactory,
        ILogger<Handler> logger,
        IFactCheckerService factCheckerService
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
            await PublishAsync(new PostCreatedEvent(req.Title, req.Content, post.Id), Mode.WaitForNone, ct);
            return TypedResults.Ok(new Response()
            {
                Id = post.Id,
            });
        }
    }
}