using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FakeoverFlow.Backend.Http.Api.Features.Auth.Available;

public static partial class Available
{
    public class Handler(
        IUserService userService,
        ILogger<Available.Handler> logger
        )
        : Endpoint<Request, Results<Ok, ErrorResponse, ProblemDetails>>
    {
        public override void Configure()
        {
            Description(x =>
            {
                x.WithName("Available");
                x.WithDescription("Checks the availability of the requested type");
            });
            
            Post("/available");
            Group<AuthGroup>();
            Validator<Validator>();
            AllowAnonymous();
            Throttle(3, 2, "X-ClientID");
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var existsResponse = await userService.CheckExistsAsync(req);
            if (existsResponse.IsFailure)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await HttpContext.Response.CompleteAsync();
                return;
            }

            var existsResponseValue = existsResponse.Value;
            if (existsResponseValue)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                await HttpContext.Response.CompleteAsync();
                return;
            }
            
            HttpContext.Response.StatusCode = StatusCodes.Status200OK;
            await HttpContext.Response.CompleteAsync();
        }
    }
}