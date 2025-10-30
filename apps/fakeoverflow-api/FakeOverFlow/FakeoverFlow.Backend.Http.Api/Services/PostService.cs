using FakeoverFlow.Backend.Abstraction;
using FakeoverFlow.Backend.Abstraction.Context;
using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FakeoverFlow.Backend.Http.Api.Models.Enums;
using FakeoverFlow.Backend.Http.Api.Models.Posts;
using Microsoft.EntityFrameworkCore;

namespace FakeoverFlow.Backend.Http.Api.Services;

public class PostService(
    AppDbContext dbContext,
    IContextFactory contextFactory,
    ILogger<PostService> logger
    ) : IPostService
{
    
    public async Task<Posts> CreatePostAsync(Features.Posts.CreatePosts.Posts.Request req, CancellationToken ct = default)
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

        await dbContext.Posts.AddAsync(post, ct);
        await dbContext.PostContent.AddAsync(content, ct);
        await dbContext.SaveChangesAsync(ct);
        
        return post;
    }

    public async Task<Result<(Posts post, PostContent? question, List<PostContent>? answers)>> GetPostByIdAsync(string id, bool includeAudit = true, bool includeQuestion = true, int fetchAnswers = 0,
        bool trackEntity = true, CancellationToken ct = default)
    {
        var normalizedId = id.ToUpper();
        var postsEnumerable = dbContext.Posts.AsQueryable();

        if (!trackEntity)
        {
            postsEnumerable = postsEnumerable.AsNoTracking();
        }

        if (includeAudit)
        {
            postsEnumerable = postsEnumerable
                .Include(x => x.CreatedByAccount)
                .Include(x => x.UpdatedByAccount);
        }

        var posts = await postsEnumerable.FirstOrDefaultAsync(x => x.Id == normalizedId, cancellationToken: ct);
        
        if (posts is null)
            return Result<(Posts post, PostContent? question, List<PostContent>? answers)>.Failure(Errors.Errors.PostErrors.NoPostFoundForId);

        var contentEnumerable = dbContext.PostContent.AsQueryable();
        if (!trackEntity)
        {
            contentEnumerable = contentEnumerable.AsNoTracking();
        }

        PostContent? question = null;

        if (includeQuestion)
        {
            question = await contentEnumerable.FirstOrDefaultAsync(x =>
                x.PostId == normalizedId && x.ContentType == ContentType.Questions, cancellationToken: ct);
        }
        
        List<PostContent>? answers = null;

        if (fetchAnswers > 0)
        {
            answers = await contentEnumerable.Where(x => x.PostId == normalizedId && x.ContentType == ContentType.Answers)
                .OrderBy(x => x.CreatedOn)
                .ToListAsync(cancellationToken: ct);
        }
        
        return Result<(Posts post, PostContent? question, List<PostContent>? answers)>.Success((posts, question, answers));
    }

    public async Task<bool> IncreaseViewCountAsync(string id, CancellationToken ct = default)
    {
        var result = await dbContext.Database.ExecuteSqlInterpolatedAsync($"""
                                                       UPDATE public."Posts" SET "Views" = "Views"  + 1 WHERE "Id" = {id.ToUpper()}
                                                       """, cancellationToken: ct);
        
        return result > 0;
    }
}