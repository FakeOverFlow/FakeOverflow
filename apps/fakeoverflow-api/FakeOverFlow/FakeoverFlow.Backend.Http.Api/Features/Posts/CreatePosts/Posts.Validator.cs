using FastEndpoints;
using FluentValidation;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts.CreatePosts;

public static partial class Posts
{
    public class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Title must be less than 500 characters");

            RuleFor(x => x.Content)
                .NotEmpty()
                .MaximumLength(10000)
                .WithMessage("Content must be less than 10000 characters");
            
            RuleFor(x => x.Tags)
                .Must(x => x.Count <= 10)
                .WithMessage("You can only add up to 10 tags");
        }
    }
}