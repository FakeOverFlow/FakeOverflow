using FakeoverFlow.Backend.Abstraction;
using FakeoverFlow.Backend.Http.Api.Models.Posts;
using Posts = FakeoverFlow.Backend.Http.Api.Features.Posts.CreatePosts.Posts;

namespace FakeoverFlow.Backend.Http.Api.Abstracts.Services;

public interface IPostService
{
    /// <summary>
    /// Creates a new post with the specified request data.
    /// </summary>
    /// <param name="request">
    /// The data required to create a new post, including its title, content, and associated tags.
    /// </param>
    /// <param name="ct">A cancellation token used to manage task cancellation.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains the created post.
    /// </returns>
    public Task<Models.Posts.Posts> CreatePostAsync(Posts.Request request, CancellationToken ct = default);

    /// <summary>
    /// Retrieves a post by its unique identifier, including optional related data such as audit information, associated question, and answers.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the post to retrieve.
    /// </param>
    /// <param name="includeAudit">
    /// Whether to include audit information of the post in the result. Defaults to true.
    /// </param>
    /// <param name="includeQuestion">
    /// Whether to include the associated question content. Defaults to true.
    /// </param>
    /// <param name="fetchAnswers">
    /// The number of associated answers to fetch. Use 0 for no answers or a positive integer for a specific count. Defaults to 0.
    /// </param>
    /// <param name="trackEntity">
    /// Whether to track the entity in the database context. Defaults to true.
    /// </param>
    /// <param name="ct">
    /// A cancellation token used to manage task cancellation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains a tuple including the post,
    /// optional associated question content, and an optional list of answers.
    /// </returns>
    public Task<Result<(Models.Posts.Posts post, PostContent? question, List<PostContent>? answers)>> GetPostByIdAsync(
        string id,
        bool includeAudit = true,
        bool includeQuestion = true,
        int fetchAnswers = 0,
        bool trackEntity = true,
        CancellationToken ct = default);

    /// <summary>
    /// Increases the view count for a specific post identified by its ID.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the post whose view count will be incremented.
    /// </param>
    /// <param name="ct">
    /// A cancellation token used to manage task cancellation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result is a boolean indicating whether the operation succeeded.
    /// </returns>
    public Task<bool> IncreaseViewCountAsync(string id, CancellationToken ct = default);

    Task<Result<(IEnumerable<(Models.Posts.Posts post, PostContent? content)> items, long totalCount)>>
        ListPostsAsync(int page, int pageSize, CancellationToken ct = default);
}