using FakeoverFlow.Backend.Abstraction.Errors;

namespace FakeoverFlow.Backend.Http.Api.Errors;

public partial class Errors
{
    public static class PostErrors
    {
        public static readonly Error NoPostFoundForId = new Error("POST_NOT_FOUND", ErrorType.NotFound, "No post found with the provided identifier(s).");
    }
}