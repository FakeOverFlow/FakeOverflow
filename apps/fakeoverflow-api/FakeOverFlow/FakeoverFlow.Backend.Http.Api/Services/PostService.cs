using FakeoverFlow.Backend.Abstraction;
using FakeoverFlow.Backend.Abstraction.Context;
using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FakeoverFlow.Backend.Http.Api.Models.Enums;
using FakeoverFlow.Backend.Http.Api.Models.Posts;
using FakeoverFlow.Backend.Http.Api.Utils;
using Microsoft.EntityFrameworkCore;

namespace FakeoverFlow.Backend.Http.Api.Services;

public class PostService(
    AppDbContext dbContext,
    IContextFactory contextFactory,
    ILogger<PostService> logger
) : IPostService
{
    public async Task<Posts> CreatePostAsync(Features.Posts.CreatePosts.Posts.Request req,
        CancellationToken ct = default)
    {
        var ctx = contextFactory.RequestContext;
        var now = DateTimeOffset.UtcNow;
        var userId = ctx.UserId;

        // Create Base Models
        var post = new Posts()
        {
            Id = Ulid.NewUlid().ToString(),
            Title = req.Title,
            Views = 0,
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
            UpdatedOn = now,
            Votes = 0,
            ContentType = ContentType.Questions,
        };

        // Handle Tags
        var tags = await GetOrCreateTagsAsync(req.Tags, ct);
        await dbContext.AddRangeAsync(tags.Select(x => new PostTags()
        {
            PostId = post.Id,
            TagId = x.Id,
        }).ToList(), ct);

        await dbContext.Posts.AddAsync(post, ct);
        await dbContext.PostContent.AddAsync(content, ct);
        await dbContext.SaveChangesAsync(ct);

        return post;
    }

    public async Task<Result<(Posts post, PostContent? question, List<PostContent>? answers)>> GetPostByIdAsync(
        string id, bool includeAudit = true, bool includeQuestion = true, int fetchAnswers = 0,
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
            return Result<(Posts post, PostContent? question, List<PostContent>? answers)>.Failure(Errors.Errors
                .PostErrors.NoPostFoundForId);

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
            answers = await contentEnumerable
                .Where(x => x.PostId == normalizedId && x.ContentType == ContentType.Answers)
                .OrderBy(x => x.CreatedOn)
                .ToListAsync(cancellationToken: ct);
        }

        return Result<(Posts post, PostContent? question, List<PostContent>? answers)>.Success((posts, question,
            answers));
    }

    public async Task<bool> IncreaseViewCountAsync(string id, CancellationToken ct = default)
    {
        var result = await dbContext.Database.ExecuteSqlInterpolatedAsync($"""
                                                                           UPDATE public."Posts" SET "Views" = "Views"  + 1 WHERE "Id" = {id.ToUpper()}
                                                                           """, cancellationToken: ct);

        return result > 0;
    }

    public async Task<Result<(IEnumerable<(Posts post, PostContent? content)> items, long totalCount)>>
        ListPostsAsync(int page, int pageSize, CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;

        var postsQuery = dbContext.Posts.AsQueryable()
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedOn);

        var totalCount = await postsQuery.LongCountAsync(ct);

        var pagedPosts = await postsQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        if (!pagedPosts.Any())
            return Result<(IEnumerable<(Posts post, PostContent? content)> items, long totalCount)>.Success((
                Enumerable.Empty<(Posts, PostContent?)>(), totalCount));

        var postIds = pagedPosts.Select(p => p.Id).ToList();

        var contents = await dbContext.PostContent
            .AsNoTracking()
            .Where(pc => postIds.Contains(pc.PostId) && pc.ContentType == ContentType.Questions)
            .ToListAsync(ct);

        var items = pagedPosts
            .Select(p =>
            {
                var content = contents.FirstOrDefault(c => c.PostId == p.Id);
                return (post: p, content: content);
            })
            .ToList();

        return Result<(IEnumerable<(Posts post, PostContent? content)> items, long totalCount)>.Success((items,
            totalCount));
    }

    private async Task<List<Tag>> GetOrCreateTagsAsync(List<string> tags, CancellationToken ct = default)
    {
        var normalizedTags = tags
            .Select(x => x.Trim().ToLower())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        var existingTags = await dbContext.Tag
            .Where(t => normalizedTags.Contains(t.Value))
            .ToListAsync(ct);

        var existingValues = existingTags.Select(t => t.Value).ToHashSet();
        var missingValues = normalizedTags.Where(v => !existingValues.Contains(v)).ToList();

        if (missingValues.Count <= 0) return existingTags;
        var newTags = missingValues.Select(v => new Tag
        {
            Value = v,
            Description = "",
            Color = TagColorPalette.GetRandomColor()
        }).ToList();

        dbContext.Tag.AddRange(newTags);
        await dbContext.SaveChangesAsync(ct);

        existingTags.AddRange(newTags);
        return existingTags;
    }
}