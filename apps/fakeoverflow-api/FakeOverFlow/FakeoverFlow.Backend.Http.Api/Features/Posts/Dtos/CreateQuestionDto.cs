using System.ComponentModel.DataAnnotations;

namespace FakeoverFlow.Backend.Http.Api.Features.Posts.Dtos
{
    public class CreateQuestionDto
    {
        [Required] [MaxLength(250)] public string Title { get; set; } = null!;

        [Required] public string Content { get; set; } = null!;

        public Guid CreatedBy { get; set; } = Guid.Empty;
    }
}