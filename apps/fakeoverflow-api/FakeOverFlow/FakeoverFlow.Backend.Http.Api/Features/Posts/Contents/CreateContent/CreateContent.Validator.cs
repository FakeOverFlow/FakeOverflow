using FastEndpoints;
using FluentValidation;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts.Contents.CreateContent;

public partial class CreateContent
{
    public class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Content)
                .NotEmpty();

            RuleFor(x => x.Content)
                .MinimumLength(4);
        }
    }
}