using FakeoverFlow.Backend.Abstraction;
using FakeoverFlow.Backend.Http.Api.Features.Auth.Available;
using FakeoverFlow.Backend.Http.Api.Features.Auth.Login;
using FakeoverFlow.Backend.Http.Api.Features.Auth.Signup;
using FakeoverFlow.Backend.Http.Api.Models.Accounts;

namespace FakeoverFlow.Backend.Http.Api.Abstracts.Services;

public interface IUserService
{
    /// <summary>
    /// Creates a new user account based on the provided signup request.
    /// </summary>
    /// <param name="request">The signup request containing user details such as email, password, first name, last name, and username.</param>
    /// <param name="verifyEmail">Verify the email along with signup</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the operation if needed.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result indicating the success or failure of creating the user account, along with the created user account details if successful.</returns>
    public Task<Result<UserAccount>> CreateUserAsync(Signup.Request request, bool verifyEmail = false,
        CancellationToken? cancellationToken = default);

    /// <summary>
    /// Retrieves an existing user verification record for the specified user, or creates a new one if none exists.
    /// </summary>
    /// <param name="userId">The unique identifier of the user for whom the verification record is being retrieved or created.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the operation if needed.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user's verification record, including the verification token and associated account details.</returns>
    public Task<Result<UserAccountVerification>> GetOrCreateUserVerificationAsync(Guid userId,
        CancellationToken? cancellationToken = default);

    /// <summary>
    /// Retrieves a user account by its unique identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to retrieve.</param>
    /// <param name="track">Indicates whether to query the user with tracking enabled. Defaults to false.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the operation if needed.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user account if found, otherwise null.</returns>
    public Task<UserAccount?> GetUserByIdAsync(Guid userId, bool track = false,
        CancellationToken? cancellationToken = default);

    /// <summary>
    /// Authenticates a user based on the provided login request.
    /// </summary>
    /// <param name="request">The login request containing authentication type, username, password, or other required data.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the operation if needed.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result indicating the success or failure of the user authentication, along with the authenticated user account details if successful.</returns>
    public Task<Result<UserAccount>> LoginAsync(Login.Request request,
        CancellationToken? cancellationToken = default);

    /// <summary>
    /// Creates a new refresh token for the specified user based on their unique identifier and name identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user for whom the refresh token will be generated.</param>
    /// <param name="jti">The name identifier associated with the user, typically used for authentication purposes.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the operation if needed.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result indicating the success or failure of creating the refresh token, along with the token if successful.</returns>
    public Task<Result<RefreshTokens>> CreateRefreshTokenForUserAsync(Guid userId, string jti,
        CancellationToken? cancellationToken = default);

    /// <summary>
    /// Deletes a refresh token associated with the specified user and token identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose refresh token is to be deleted.</param>
    /// <param name="jti">The unique token identifier of the refresh token to be deleted.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the operation if needed.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task DeleteRefreshTokenAsync(Guid userId, string jti, CancellationToken? cancellationToken = default);

    /// <summary>
    /// Retrieves the refresh token for a specified user and token identifier with optional validation and revocation checks.
    /// </summary>
    /// <param name="userId">The unique identifier of the user associated with the refresh token.</param>
    /// <param name="jti">The unique token identifier (JTI) associated with the refresh token.</param>
    /// <param name="track">Indicates whether the refresh token entity should be tracked in the current context.</param>
    /// <param name="checkExpiry">Determines whether to validate the expiration status of the refresh token.</param>
    /// <param name="checkRevoked">Determines whether to validate if the refresh token has been revoked.</param>
    /// <param name="revokeOnCall">Indicates whether to revoke the refresh token upon retrieval.</param>
    /// <param name="cancellationToken">An optional token to cancel the operation if required.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the result of the refresh token retrieval, including validation or revocation status.</returns>
    public Task<Result<RefreshTokens>> GetRefreshTokenOf(Guid userId, string jti, bool track = false,
        bool checkExpiry = true,
        bool checkRevoked = true, bool revokeOnCall = false,
        CancellationToken? cancellationToken = default);

    /// <summary>
    /// Checks the existence of a user-related attribute, such as a username, based on the provided request.
    /// </summary>
    /// <param name="request">The request object containing the type of attribute to check (e.g., username) and its value.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the operation if needed.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a result object indicating whether the attribute exists.</returns>
    public Task<Result<bool>> CheckExistsAsync(Available.Request request,
        CancellationToken? cancellationToken = default);
}