using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FakeoverFlow.Backend.Http.Api.Extensions;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts.Contents.Votes.DeleteVote;

public partial class DeleteVote
{
    public class Handler(
        IPostService postService,
        ILogger<DeleteVote> logger
    ) : Endpoint<Request, Results<NoContent, ErrorResponse, ProblemDetails>>
    {
        public override async Task<Results<NoContent, ErrorResponse, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
        {
            req.PostId = HttpContext.Request.RouteValues["postId"]?.ToString() ?? string.Empty;
            req.ContentId = Guid.TryParse(HttpContext.Request.RouteValues["contentId"]?.ToString() ?? string.Empty, out var ctId) ? ctId : Guid.Empty;
            
            if(string.IsNullOrWhiteSpace(req.PostId))
                AddError(new ValidationFailure(nameof(req.PostId), $"PostId is required"));
            
            if(Guid.Empty == req.ContentId)
                AddError(new ValidationFailure(nameof(req.ContentId), $"ContentId is required"));

            ThrowIfAnyErrors();

            var result = await postService.DeleteVoteAsync(req.PostId, req.ContentId, ct);
            if (result.IsFailure)
                return result.Error!.ToFastEndpointError();
            
            return TypedResults.NoContent();
        }

        public override void Configure()
        {
            Delete("");
            Group<VoteGroup>();
            Description(x => x.WithName("DeleteVote"));
        }
    }
}