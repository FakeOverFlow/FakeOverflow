using FakeoverFlow.Backend.Abstraction.Context;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using PostsModel = FakeoverFlow.Backend.Http.Api.Models.Posts.Posts;
using PostContentModel = FakeoverFlow.Backend.Http.Api.Models.Posts.PostContent;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts.Post;

public static partial class Posts
{
    public class Handler(
        AppDbContext db,
        IContextFactory contextFactory
    ) : Endpoint<Request, Results<Ok<Response>, BadRequest<InvalidResponse>, ProblemHttpResult>>
    {
        public override void Configure()
        {
            Description(x => x.WithName("CreatePost"));

            Post("");
            AllowAnonymous();
            Group<PostGroup>();
        }

        public override async Task<Results<Ok<Response>, BadRequest<InvalidResponse>, ProblemHttpResult>> ExecuteAsync(
            Request req,
            CancellationToken ct)
        {
            var ctx = contextFactory.RequestContext;


            /*if (!ctx.IsAuthenticated)
            {
                return TypedResults.Problem(
                    detail: "Authentication required to create a post.",
                    title: "Unauthorized",
                    statusCode: StatusCodes.Status401Unauthorized
                );
            }*/

            if (string.IsNullOrWhiteSpace(req.Title) || string.IsNullOrWhiteSpace(req.Content))
            {
                return TypedResults.BadRequest(new InvalidResponse { Message = "Title and Content are required." });
            }

            var now = DateTimeOffset.UtcNow;

            var post = new PostsModel
            {
                Id = Guid.NewGuid().ToString(),
                Title = req.Title,
                Views = 0,
                Votes = 0,
                CreatedBy = req.CreatedBy,
                CreatedOn = now,
                UpdatedBy = req.CreatedBy,
                UpdatedOn = now,
            };

            var content = new PostContentModel
            {
                Id = Guid.NewGuid(),
                PostId = post.Id,
                Content = req.Content,
                CreatedBy = req.CreatedBy,
                CreatedOn = now,
                UpdatedBy = req.CreatedBy,
                UpdatedOn = now
            };

            try
            {
                await db.Set<PostsModel>().AddAsync(post, ct);
                await db.Set<PostContentModel>().AddAsync(content, ct);
                await db.SaveChangesAsync(ct);

                var response = new Response
                {
                    Id = post.Id,
                    Title = post.Title,
                    Content = content.Content,
                    CreatedBy = post.CreatedBy
                };

                return TypedResults.Ok(response);
            }
            catch (Exception ex)
            {
                // return problem result that matches the Results<> generic
                return TypedResults.Problem(
                    detail: ex.Message,
                    title: "An error occurred creating the post",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }
    }
}