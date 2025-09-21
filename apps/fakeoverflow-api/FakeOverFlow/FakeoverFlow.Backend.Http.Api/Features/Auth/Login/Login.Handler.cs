using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FakeoverFlow.Backend.Http.Api.Features.Auth.Login;

internal partial class Login 
{
    public class Handler : Endpoint<Request,
    Results<Ok<Response>, ErrorResponse, ProblemDetails>
    >
    {
        public override void Configure()
        {
            Description(x =>
            {
                x.Produces<Ok<Response>>(200);
                x.Produces<ErrorResponse>(400);
            });
            Post("/login");
            AllowAnonymous();
            Group<AuthGroup>();
            Validator<Validator>();
        }

        public async override Task<Results<Ok<Response>, ErrorResponse, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
        {
            var x = TypedResults.Ok(new Login.Response());
            return x;
        }
    }
}