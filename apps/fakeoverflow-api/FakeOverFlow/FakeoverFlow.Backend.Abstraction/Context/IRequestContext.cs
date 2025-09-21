namespace FakeoverFlow.Backend.Abstraction.Context;

public interface IRequestContext
{
    /// <summary>
    /// Gets a value indicating whether the current request context
    /// is associated with an authenticated user.
    /// </summary>
    /// <remarks>
    /// This property is used to determine if a user has been successfully
    /// authenticated. An authenticated user is typically associated with a
    /// valid identity. If false, the user is not authenticated, and other
    /// user-related properties such as <c>UserId</c>, <c>Role</c>, or <c>Email</c>
    /// may not contain meaningful values.
    /// </remarks>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the unique identifier of the authenticated user in the current request context.
    /// </summary>
    /// <remarks>
    /// This property provides the <c>Guid</c> identifier associated with the authenticated user.
    /// It can be used to track, audit, or associate actions with a specific user in the application.
    /// If <c>IsAuthenticated</c> is false, this property may return a default or empty value.
    /// </remarks>
    Guid UserId { get; }

    /// <summary>
    /// Gets the role of the current user within the context of authorization and access control.
    /// </summary>
    /// <remarks>
    /// This property represents the user's assigned role, which determines the level of permissions
    /// and access they have. Common roles may include "Admin", "Moderator", and "User". The role value
    /// is typically retrieved from the user's claims and used throughout the application to enforce
    /// role-based security policies.
    /// </remarks>
    string Role { get; }

    /// <summary>
    /// Gets a value indicating whether the current user has administrative privileges.
    /// </summary>
    /// <remarks>
    /// This property evaluates if the user's role corresponds to an administrative role.
    /// It is typically used to determine if the user is authorized to perform actions
    /// that require elevated permissions. If true, the user possesses admin capabilities;
    /// otherwise, the user does not have administrative access.
    /// </remarks>
    bool IsAdmin { get; }

    /// <summary>
    /// Gets the email address associated with the current user.
    /// </summary>
    /// <remarks>
    /// This property provides the email address obtained from the authenticated user's claims.
    /// If the user is not authenticated, this property may return an empty string or a null value
    /// depending on the context implementation.
    /// </remarks>
    string Email { get; }

    /// <summary>
    /// Gets the user name associated with the current request context.
    /// </summary>
    /// <remarks>
    /// This property provides the name of the user as retrieved from the authentication system.
    /// It is typically sourced from the <c>ClaimsPrincipal</c> of the authenticated user.
    /// If the user is not authenticated, this property may return an empty string or
    /// a default value, depending on the implementation.
    /// </remarks>
    string UserName { get; }

    /// <summary>
    /// Gets the first name of the authenticated user, if available.
    /// </summary>
    /// <remarks>
    /// This property provides the given name (first name) of the user as defined
    /// within the associated claims or context. If the user is not authenticated
    /// or the claims do not contain this information, it typically returns an
    /// empty string.
    /// </remarks>
    string FirstName { get; }

    /// <summary>
    /// Gets the last name of the user associated with the current request context.
    /// </summary>
    /// <remarks>
    /// This property represents the surname or family name of the authenticated user.
    /// It is typically retrieved from the user's identity claims or a similar data source.
    /// If the user is not authenticated, this property may return an empty string or
    /// a default value.
    /// </remarks>
    string LastName { get; }

    /// <summary>
    /// Gets the token identifier associated with the current request context.
    /// </summary>
    /// <remarks>
    /// This property represents a unique identifier for the token used in the
    /// current request. It is typically utilized for tracking or validation purposes
    /// within the system. The value of this property can be used to relate the request
    /// to a specific user session or security context.
    /// </remarks>
    string TokenId { get; }
}