using FakeoverFlow.Backend.Abstraction.Errors;
using FastEndpoints;

namespace FakeoverFlow.Backend.Http.Api.Extensions;

public static class ResultExtensions
{
    public static ErrorResponse ToFastEndpointError(this Error error)
    {
        return new ErrorResponse
        {
            Message = string.Join(Environment.NewLine, error.Description),
            StatusCode = MapStatusCode(error.Type)
        };
    }

    public static ErrorResponse FastEndpointProblem(this Error error)
    {
        return error.ToFastEndpointError();
    }


    private static int MapStatusCode(ErrorType type) => type switch
    {
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.BadRequest => StatusCodes.Status400BadRequest,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.ServerError => StatusCodes.Status500InternalServerError,
        ErrorType.Unknown => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status500InternalServerError
    };
}