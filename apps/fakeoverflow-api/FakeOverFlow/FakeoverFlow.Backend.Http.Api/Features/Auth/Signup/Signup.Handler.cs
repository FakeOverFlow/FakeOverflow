using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FakeoverFlow.Backend.Http.Api.Events.Models;
using FakeoverFlow.Backend.Http.Api.Extensions;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FakeoverFlow.Backend.Http.Api.Features.Auth.Signup;

public static partial class Signup
{
    public class Handler(IUserService userService)
        : Endpoint<Signup.Request, Results<Created<Signup.Response>, ErrorResponse, ProblemDetails>>
    {
        public override void Configure()
        {
            Description(x =>
            {
                x.Produces<Response>(201);
                x.Produces<ProblemDetails>(400);
                x.Produces<ProblemDetails>(401);
                x.Produces<ProblemDetails>(404);
                x.Produces<ProblemDetails>(409);
                x.Produces<ProblemDetails>(500);
            });

            Post("/signup");
            Group<AuthGroup>();
            Validator<Validator>();
        }

        public override async Task<Results<Created<Response>, ErrorResponse, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
        {
            var userCreateRst = await userService.CreateUserAsync(req, cancellationToken: ct);
            if(userCreateRst.IsSuccess)
                return TypedResults.Created("/me", new Response()
                {
                    UserId = userCreateRst.Value!.Id,
                    Email = userCreateRst.Value.Email,
                });

            await PublishAsync(new UserSignupEvent(userCreateRst.Value!.Id, userCreateRst.Value.Email),
                Mode.WaitForNone, cancellation: ct);
            return userCreateRst.Error!.ToFastEndpointError();
        }
    }

}