using FakeoverFlow.Backend.Abstraction.Context;
using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FakeoverFlow.Backend.Http.Api.Extensions;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts.ViewPost;

public static partial class ViewPost
{
    public class Handler(
        ILogger<Handler> logger,
        IContextFactory contextFactory,
        IPostService postService
    ) : EndpointWithoutRequest<Results<Ok<Response>, ErrorResponse, ProblemDetails>>
    {
        public override void Configure()
        {
            Get("/{id}");
            Group<PostGroup>();
            Description(x => { x.WithName("GetPostById"); });
            AllowAnonymous();
        }

        public override async Task<Results<Ok<Response>, ErrorResponse, ProblemDetails>> ExecuteAsync(
            CancellationToken ct)
        {
            if (!HttpContext.Request.RouteValues.TryGetValue("id", out var id))
            {
                AddError(new ValidationFailure("id", "id is required"));
                ThrowIfAnyErrors();
            }

            var postResult = await postService.GetPostByIdAsync(id.ToString(), true, true, 0, ct: ct);
            if (postResult.IsFailure)
                return postResult.Error!.ToFastEndpointError();

            var postResultValue = postResult.Value!;
            var response = new Response()
            {
                PostId = postResultValue.post.Id,
                Content = postResultValue.question!.Content,
                Title = postResultValue.post.Title,
                Tags = [],
                Views = postResultValue.post.Views,
                Votes = postResultValue.question.Votes,
                CreatedOn = new UserActivity()
                {
                    User = new UserDetails()
                    {
                        Id = postResultValue.post.CreatedBy,
                        Name = postResultValue.post.CreatedByAccount?.Username ?? string.Empty,
                        ProfilePicture = postResultValue.post.CreatedByAccount?.ProfileImageUrl ?? string.Empty,
                    },
                    ActivityOn = postResultValue.post.CreatedOn,
                },
            };

            if (postResultValue.post.UpdatedBy != postResultValue.post.CreatedBy ||
                postResultValue.post.UpdatedOn != postResultValue.post.CreatedOn)
            {
                response.UpdatedOn = new UserActivity()
                {
                    User = new UserDetails()
                    {
                        Id = postResultValue.post.UpdatedBy,
                        Name = postResultValue.post.UpdatedByAccount?.Username ?? string.Empty,
                        ProfilePicture = postResultValue.post.UpdatedByAccount?.ProfileImageUrl ?? string.Empty,
                    },
                    ActivityOn = postResultValue.post.UpdatedOn,
                };
            }

            bool increaseViews = !contextFactory.RequestContext.IsAuthenticated ||
                                 (contextFactory.RequestContext.UserId == postResultValue.post.CreatedBy);
            if (increaseViews)
            {
                logger.LogTrace("Increasing view count for post {PostId}", id);
                var viewIncrease = await postService.IncreaseViewCountAsync(id.ToString()!, ct);
                if (!viewIncrease)
                {
                    logger.LogError("Failed to increase view count");
                }
            }

            return TypedResults.Ok(response);
        }
    }
}