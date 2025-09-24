using System.Text;
using FakeoverFlow.Backend.Abstraction;
using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FakeoverFlow.Backend.Http.Api.Features.Auth.Login;
using FakeoverFlow.Backend.Http.Api.Features.Auth.Signup;
using FakeoverFlow.Backend.Http.Api.Models.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FakeoverFlow.Backend.Http.Api.Services;

public class UserService(
    AppDbContext dbContext,
    IPasswordHasher<UserAccount> passwordHasher,
    ILogger<UserService> logger
    )
    : IUserService
{
    
    /// <inheritdoc />
    public async Task<Result<UserAccount>> CreateUserAsync(Signup.Request request, bool verifyEmail = false, CancellationToken? cancellationToken = default)
    {
        try
        {
            logger.LogDebug("Trying to create a new user account for {Email} with username {Username}", request.Email,
                request.Username);
            var id = Guid.CreateVersion7();
            var user = new UserAccount()
            {
                Id = id,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CreatedBy = id,
                CreatedOn = DateTimeOffset.UtcNow,
                ProfileImageUrl = null,
                IsDisabled = false,
                IsDeleted = false,
                Username = request.Username,
                UpdatedBy = id,
                UpdatedOn = DateTimeOffset.UtcNow,
                VerifiedOn = verifyEmail ? DateTimeOffset.UtcNow : null,
                Settings = new UserAccountSettings()
            };
            var hashedPassword = passwordHasher.HashPassword(user, request.Password);
            var passwordBytes = Encoding.UTF8.GetBytes(hashedPassword);
            user.Password = passwordBytes;

            dbContext.UserAccounts.Add(user);
            await dbContext.SaveChangesAsync();
            return Result<UserAccount>.Success(user);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to create the user");
            return Result<UserAccount>.Failure(Errors.Errors.UnknownError);
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="userId"><inheritdoc/></param>
    /// <param name="cancellationToken"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public async Task<Result<UserAccountVerification>> GetOrCreateUserVerificationAsync(Guid userId, CancellationToken? cancellationToken = default)
    {
        logger.LogDebug("Trying to get or create a user verification record for user {UserId}", userId);
        var verifications = await dbContext.UserAccountVerifications.Where(x => x.UserId == userId)
            .Include(x => x.Account)
            .Select(x => new
            {
                x.VerificationToken, x.Account.Email, x.Account.FirstName, x.Account.LastName, x.Account.Username,
                x.Account.IsDisabled, x.Account.IsDeleted
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (verifications is null)
            return await CreateUserVerificationAsync(userId, cancellationToken);

        if (verifications.IsDisabled || verifications.IsDeleted)
            return Result<UserAccountVerification>.Failure(Errors.Errors.UserAccounts.AccountDisabled);
        
        return Result<UserAccountVerification>.Success(new UserAccountVerification()
        {
            UserId = userId,
            VerificationToken = verifications.VerificationToken,
        });
    }

    public async Task<UserAccount?> GetUserByIdAsync(Guid userId, bool track = false, CancellationToken? cancellationToken = default)
    {
        var userAccounts = dbContext
            .UserAccounts
            .AsQueryable();

        if (!track)
        {
            logger.LogDebug("Disabling tracking for user {UserId}", userId);
            userAccounts = userAccounts.AsNoTracking();
        }

        var user = await userAccounts.Where(x => x.Id == userId).FirstOrDefaultAsync();
        return user;
    }

    public async Task<Result<UserAccount>> LoginAsync(Login.Request request, CancellationToken? cancellationToken = default)
    {
        var userAccounts = dbContext
            .UserAccounts
            .AsNoTracking();
        
        
        if (request.Type == Login.AuthenticationType.Credentials)
        {
            userAccounts = userAccounts.Where(x => x.Email == request.UserName || x.Username == request.UserName);   
        } else if (request.Type == Login.AuthenticationType.Google)
        {
            //TODO
        }

        var account = await userAccounts.FirstOrDefaultAsync();

        if (account is null)
        {
            // If no account if its username/password login, then stop the exectution
            if(request.Type == Login.AuthenticationType.Credentials)
                return Result<UserAccount>.Failure(Errors.Errors.UserAccounts.NotFound);

            // If no account if its google login, then create a new account
            var userCreationResult = await CreateUserAsync(new Signup.Request()
            {
                Email = string.Empty,
                Password = null,
                Username = request.UserName,
                FirstName = string.Empty,
                LastName = string.Empty,
            }, verifyEmail: true, cancellationToken: cancellationToken);
            
            // Stop execution if the user creation failed
            if (!userCreationResult.IsSuccess)
            {
                logger.LogError("Failed to create a new user account for google login due to {Error}", string.Join(", ", userCreationResult.Error!.Description));
                return Result<UserAccount>.Failure(Errors.Errors.UserAccounts.FailedToCreateOAuthAccount);
            }

            return userCreationResult;
        }

        if (account.IsDisabled || account.IsDeleted)
        {
            logger.LogTrace("User {UserId} is disabled", account.Id);
            return Result<UserAccount>.Failure(Errors.Errors.UserAccounts.AccountDisabled);
        }

        if (account.Password is null)
        {
            logger.LogWarning("User is signed up using GoogleOAuth");
            return Result<UserAccount>.Failure(Errors.Errors.UserAccounts.InvalidLoginMethod);
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            logger.LogWarning("Password is empty");
            return Result<UserAccount>.Failure(Errors.Errors.UserAccounts.InvalidLoginMethod);
        }

        var passwordString = Encoding.UTF8.GetString(account.Password);
        var isPasswordValid = passwordHasher.VerifyHashedPassword(account, passwordString, request.Password) == PasswordVerificationResult.Success;
        
        if(isPasswordValid)
            return Result<UserAccount>.Success(account);
        
        //TODO Update last invalid login date and attempt count and maybe block the account for a while
        // dbContext.UserAccounts.FromSqlInterpolated(
        //     "UPDATE UserAccounts SET LastInvalidLoginDate = {now}, AttemptCount = AttemptCount + 1 WHERE Id = {userId}");
        
        return Result<UserAccount>.Failure(Errors.Errors.UserAccounts.InvalidCredentials);
    }

    private async Task<Result<UserAccountVerification>> CreateUserVerificationAsync(Guid userId, CancellationToken? cancellationToken = default)
    {
        logger.LogDebug("Trying to create a user verification record for user {UserId}", userId);
        try
        {
            var verification = new UserAccountVerification()
            {
                VerificationToken = Ulid.NewUlid().ToString(),
                UserId = userId,
                CreatedOn = DateTimeOffset.UtcNow,
            };
            dbContext.UserAccountVerifications.Add(verification);
            logger.LogDebug("User verification record created for user {UserId}, {VerificationToken}", userId,
                verification.VerificationToken);
            await dbContext.SaveChangesAsync();
            
            return Result<UserAccountVerification>.Success(verification);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating user verification record for user {UserId}", userId);
            return Result<UserAccountVerification>.Failure(Errors.Errors.UnknownError);
        }
    }
}