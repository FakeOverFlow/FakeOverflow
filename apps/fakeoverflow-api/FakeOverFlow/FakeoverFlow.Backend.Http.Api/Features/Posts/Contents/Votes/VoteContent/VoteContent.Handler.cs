using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FakeoverFlow.Backend.Http.Api.Extensions;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts.Contents.Votes.VoteContent;

public partial class VoteContent
{
    public class Handler(
        IPostService postService,
        ILogger<Handler> logger
    ) : Endpoint<VoteContent.Request, Results<Ok<VoteContent.Response>, ErrorResponse, ProblemHttpResult>>
    {
        public override async Task<Results<Ok<VoteContent.Response>, ErrorResponse, ProblemHttpResult>> ExecuteAsync(VoteContent.Request req, CancellationToken ct)
        {
            req.PostId = HttpContext.Request.RouteValues["postId"]?.ToString() ?? string.Empty;
            req.ContentId = Guid.TryParse(HttpContext.Request.RouteValues["contentId"]?.ToString() ?? string.Empty, out var ctId) ? ctId : Guid.Empty;
            
            if(string.IsNullOrWhiteSpace(req.PostId))
                AddError(new ValidationFailure(nameof(req.PostId), $"PostId is required"));
            
            if(Guid.Empty == req.ContentId)
                AddError(new ValidationFailure(nameof(req.ContentId), $"ContentId is required"));

            ThrowIfAnyErrors();

            var result = await postService.UpsertContentVotesAsync(req, ct);

            if (result.IsFailure)
                return result.Error!.ToFastEndpointError();

            return TypedResults.Ok(new Response()
            {
                ContentId = result!.Value!.ContentId,
                UserId = result!.Value!.UserId
            });
        }

        public override void Configure()
        {
            Post("");
            Group<VoteGroup>();
            Description(x => { x.WithName("VoteContent").WithSummary("Vote for a content"); });
            Throttle(3, 2, "X-ClientID");
        }
    }
}