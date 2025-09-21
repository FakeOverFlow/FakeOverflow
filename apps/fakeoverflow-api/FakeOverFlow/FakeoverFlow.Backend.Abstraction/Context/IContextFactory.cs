using Microsoft.AspNetCore.Http;

namespace FakeoverFlow.Backend.Abstraction.Context;

/// <summary>
/// Defines a factory for creating and managing the context used throughout the application.
/// </summary>
/// <remarks>
/// This interface is responsible for providing access to the current request context and HTTP context.
/// It abstracts the creation and management of <see cref="IRequestContext"/> and integrates with the
/// <see cref="IHttpContextAccessor"/> to ensure that the appropriate context is available for use during the HTTP request lifecycle.
/// </remarks>
public interface IContextFactory
{
    /// <summary>
    /// Represents the request context interface providing essential information about the current user's session and authentication state.
    /// </summary>
    /// <remarks>
    /// This property is part of the <see cref="IContextFactory"/> and provides access to user-specific details such as authentication status,
    /// user identifiers, roles, and other relevant metadata required for the lifecycle of an HTTP request.
    /// The <see cref="RequestContext"/> is used to facilitate identity-based operations and ensure seamless interaction between
    /// the application and its users.
    /// </remarks>
    IRequestContext RequestContext { get; }

    /// <summary>
    /// Provides access to the current <see cref="HttpContext"/> for the application's HTTP request lifecycle.
    /// </summary>
    /// <remarks>
    /// The <see cref="ContextAccessor"/> property is utilized to retrieve or manipulate the current <see cref="HttpContext"/> via the
    /// <see cref="IHttpContextAccessor"/>. This property acts as a bridge for accessing HTTP-specific data, enabling seamless interaction
    /// with the underlying HTTP infrastructure. It ensures that the relevant request context is available when required,
    /// supporting operations such as authentication, user identity resolution, and request-specific metadata handling.
    /// </remarks>
    IHttpContextAccessor ContextAccessor { get; }
}