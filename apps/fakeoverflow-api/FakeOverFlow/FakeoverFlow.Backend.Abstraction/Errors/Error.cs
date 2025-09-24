namespace FakeoverFlow.Backend.Abstraction.Errors;

public enum ErrorType { NotFound, Validation, Unauthorized, Unknown, ServerError, Conflict, BadRequest }

public record Error(string Id, ErrorType Type, string[] Description, string? Detail = null)
{
    public Error(string id, ErrorType type, string description, string? detail = null) : this(id, type, [description], detail) { }
}