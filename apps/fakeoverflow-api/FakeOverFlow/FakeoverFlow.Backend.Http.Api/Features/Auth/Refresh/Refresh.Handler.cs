using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FakeoverFlow.Backend.Http.Api.Features.Auth.Refresh;

internal partial class Refresh
{
    public class Handler : EndpointWithoutRequest<Response>
    {
        public override void Configure()
        {
            Description(x =>
            {
                x.WithName("Refresh");
                x.Produces<Ok<Response>>(200);
                x.Produces<ErrorResponse>(400);
            });
            Post("/refresh");
            Group<AuthGroup>();
        }

        public async override Task<Response> ExecuteAsync(CancellationToken ct)
        {
            return await base.ExecuteAsync(ct);
        }
    }
}