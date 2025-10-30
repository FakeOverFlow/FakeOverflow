using FakeoverFlow.Backend.Abstraction.Context;
using FakeoverFlow.Backend.Http.Api.Models.Posts;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts.CreatePosts;

public static partial class Posts
{
    public class Handler(
        AppDbContext db,
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
            var ctx = contextFactory.RequestContext;
            var now = DateTimeOffset.UtcNow;
            var userId = ctx.UserId;
            var post = new Models.Posts.Posts()
            {
                Id = Ulid.NewUlid().ToString(),
                Title = req.Title,
                Views = 0,
                Votes = 0,
                CreatedBy = userId,
                CreatedOn = now,
                UpdatedBy = userId,
                UpdatedOn = now,
            };

            var content = new PostContent()
            {
                Id = Guid.NewGuid(),
                PostId = post.Id,
                Content = req.Content,
                CreatedBy = userId,
                CreatedOn = now,
                UpdatedBy = userId,
                UpdatedOn = now
            };

            await db.Posts.AddAsync(post, ct);
            await db.PostContent.AddAsync(content, ct);
            await db.SaveChangesAsync(ct);

            var response = new Response
            {
                Id = post.Id,
            };

            return TypedResults.Ok(response);
        }
    }
}