using FakeoverFlow.Backend.Abstraction.Context;

namespace FakeoverFlow.Backend.Http.Api.Context;

/// <summary>
/// Factory class for creating and managing the request context within an HTTP application.
/// </summary>
/// <remarks>
/// Provides functionality to build an instance of <see cref="IRequestContext"/> based on the current
/// <see cref="IHttpContextAccessor"/>. It ensures that the appropriate <see cref="IRequestContext"/> is injected
/// depending on the state of the HTTP context, such as authenticated or non-authenticated requests.
/// </remarks>
public class ContextFactory : IContextFactory
{
    public ContextFactory(IHttpContextAccessor httpContextAccessor)
    {
        ContextAccessor = httpContextAccessor;
        RequestContext = CreateRequestContext();
    }
    
    public IRequestContext RequestContext { get; }
    public IHttpContextAccessor ContextAccessor { get; }

    private IRequestContext CreateRequestContext()
    {
        var ctx = ContextAccessor.HttpContext;
        if (ctx == null || string.IsNullOrEmpty(ctx.Request.Headers.Authorization)) return new NonValidateRequestContext();

        var ctxUser = ctx.User;
        return new RequestContext(ctxUser);
    }
}