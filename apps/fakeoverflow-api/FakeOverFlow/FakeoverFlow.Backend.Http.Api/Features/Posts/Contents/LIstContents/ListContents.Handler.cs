using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FakeoverFlow.Backend.Http.Api.Models.Enums;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts.Contents.ListContents;

public partial class ListContents
{
    public class Handler(
        IPostService postService,
        ILogger<ListContents> logger
    ) : Endpoint<Request, Results<Ok<Response>, BadRequest<ErrorResponse>, ProblemHttpResult>>
    {
        public override async Task<Results<Ok<Response>, BadRequest<ErrorResponse>, ProblemHttpResult>> ExecuteAsync(
            Request req, CancellationToken ct)
        {
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

            req.PostId = postId!;
            var postContents = await postService.GetPostContentByPostIdAsync(req.PostId, ct);
            return TypedResults.Ok(new Response()
            {
                Answers = postContents.Item1.Select(x => new PostContent()
                {
                    Content = x.Content,
                    Id = x.Id,
                    PostId = x.PostId,
                    IsInternal = x.ContentType == ContentType.Analysis,
                    CreatedOn = new UserActivity()
                    {
                        ActivityOn = x.CreatedOn,
                        UserId = x.CreatedByAccount.Id,
                        Username = x.CreatedByAccount.Username
                    },
                    UserVote = x.VotesBy.Select(x => new UserVote()
                    {
                        IsUpvote = x.UpVote
                    }).FirstOrDefault(),
                    UpVotes = postContents.Item2.GetValueOrDefault(x.Id, (0, 0)).Upvote,
                    DownVotes = postContents.Item2.GetValueOrDefault(x.Id, (0, 0)).Downvote
                }).ToList()
            });
        }

        public override void Configure()
        {
            Get("");
            AllowAnonymous();
            Description(x => { x.WithName("ListAnswers").WithSummary("Lists all answers"); });
            Group<ContentsGroup>();
        }
    }
}