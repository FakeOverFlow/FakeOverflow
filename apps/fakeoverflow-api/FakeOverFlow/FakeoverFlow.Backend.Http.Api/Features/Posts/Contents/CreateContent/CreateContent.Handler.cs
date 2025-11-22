using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts.Contents.CreateContent;

public partial class CreateContent
{
    public class Handler(
        IPostService postService,
        ILogger<CreateContent> logger
    ) : Endpoint<Request, Results<Ok<Response>, BadRequest<ErrorResponse>, ProblemHttpResult>>
    {
        public override void Configure()
        {
            Post("");
            Group<ContentsGroup>();
            Description(x => x.WithName("CreateAnswer"));
        }

        public override async Task<Results<Ok<Response>, BadRequest<ErrorResponse>, ProblemHttpResult>> ExecuteAsync(
            Request req, CancellationToken ct)
        {
            logger.LogInformation("Creating answer");
            if (!HttpContext.Request.RouteValues.TryGetValue("postId", out object? postIdRaw))
            {
                AddError(">" + (postIdRaw?.ToString() ?? "N/A") + "< is not a valid postId");
                ThrowIfAnyErrors();
            }

            var postId = postIdRaw!.ToString();
            if (string.IsNullOrWhiteSpace(postId))
            {
                AddError(">" + (postIdRaw?.ToString() ?? "N/A") + "< is not a valid postId");
                ThrowIfAnyErrors();
            }

            req.PostId = postId!.ToUpper();

            var content = await postService.CreatePostContentAsync(req, ct);

            return TypedResults.Ok(new Response()
            {
                PostId = content.PostId,
                Id = content.Id
            });
        }
    }
}