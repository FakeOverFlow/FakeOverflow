using FakeoverFlow.Backend.Abstraction;
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
    
    
}