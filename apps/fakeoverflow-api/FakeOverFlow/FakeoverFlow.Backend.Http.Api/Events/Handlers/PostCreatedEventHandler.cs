using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FakeoverFlow.Backend.Http.Api.Events.Models;
using FakeoverFlow.Backend.Http.Api.Features.Posts.Contents.CreateContent;
using FastEndpoints;

namespace FakeoverFlow.Backend.Http.Api.Events.Handlers;

public class PostCreatedEventHandler(
    ILogger<PostCreatedEventHandler> logger,
    IFactCheckerService factCheckerService,
    IServiceScopeFactory factory
    ) : IEventHandler<PostCreatedEvent>
{
    public async Task HandleAsync(PostCreatedEvent eventModel, CancellationToken ct)
    {
        try
        {
            var response = await factCheckerService.CheckFactAsync(eventModel.PostTitle, eventModel.PostContent, cancellationToken: ct);
            if (!response.Success || string.IsNullOrWhiteSpace(response.Verdict))
            {
                logger.LogWarning("Fact check failed for post {PostId}", eventModel.PostId);
                return;
            }

            using var scope = factory.CreateScope();
            var postService = scope.ServiceProvider.GetRequiredService<IPostService>();
            await postService.CreatePostContentAsync(new CreateContent.Request()
            {
                PostId = eventModel.PostId,
                Content = response.Verdict,
                IsInternal = true
            }, ct);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error checking the facts");
        }
    }
}