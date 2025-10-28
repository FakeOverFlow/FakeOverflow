using FakeoverFlow.Backend.Http.Api.Models.Posts;
using PostsModel = FakeoverFlow.Backend.Http.Api.Models.Posts.Posts;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts
{
    public interface IPostsRepository
    {
        Task<PostsModel> AddQuestionAsync(PostsModel post, PostContent content);
        Task<PostsModel?> GetByIdAsync(string id);
    }
}